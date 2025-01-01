using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Repositories.Interfaces;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DemoShop.Manager.Services
{
    public class AuthenticateUserService : IAuthenticateUserService
    {
        #region Global variables
        private readonly IUserRepository _userRepository;
        private readonly string _jwtKey;
        private static readonly Dictionary<string, int> _loginAttempts = new();
        private static readonly Dictionary<string, DateTime> _lockoutEndTime = new();
        private const int MaxAttempts = 3;
        private const int LockoutDurationMinutes = 7;
        private readonly string _resetPasswordURL;
        private static readonly Dictionary<string, (string Token, DateTime Expiration)> _resetTokens = new();
        private readonly IEmailService _emailService;
        private const int TokenExpirationMinutes = 25;
        #endregion

        #region Constructors
        public AuthenticateUserService(IConfiguration config, IUserRepository userRepository, IEmailService emailService)
        {
            _jwtKey = config["Jwt:Key"];
            _resetPasswordURL = config["ResetPassword:URL"];
            _userRepository = userRepository;
            _emailService = emailService;
            Log.Information("UserService initialized.");
        }
        #endregion

        #region Public functions
        public string Authenticate(string username, string password)
        {
            if (_lockoutEndTime.TryGetValue(username, out var lockoutEnd) && DateTime.UtcNow < lockoutEnd)
            {
                throw new InvalidOperationException("Account locked. Try again later.");
            }

            var user = _userRepository.GetByUsername(username);
            if (user == null || user.PasswordHash != HashPassword(password))
            {
                TrackFailedLogin(username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            ResetLoginAttempts(username);
            return GenerateJwtToken(user);
        }


        public void Register(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new ArgumentException("Username and password are required.");
            }

            if (_userRepository.GetByUsername(user.Username) != null)
            {
                throw new InvalidOperationException("Username is already taken.");
            }

            if (!IsValidPassword(user.PasswordHash))
            {
                throw new ArgumentException("Password must be at least 8 characters long and contain an uppercase letter, lowercase letter, number, and special character.");
            }

            user.Role = "User";
            user.PasswordHash = HashPassword(user.PasswordHash);
            _userRepository.Add(user);
            Log.Information("User registered successfully: {Username}", user.Username);
        }
        public void RequestPasswordReset(string email)
        {
            var user = _userRepository.GetByUsername(email);
            if (user == null)
            {
                throw new InvalidOperationException("No account associated with this email.");
            }

            var token = GenerateResetToken();
            _resetTokens[email] = (token, DateTime.UtcNow.AddMinutes(TokenExpirationMinutes));
            Log.Information("Password reset token generated for {Email}", email);

            var resetLink = $"{_resetPasswordURL}?token={token}";
            _emailService.Send(email, "Password Reset Request", $"Click the link to reset your password: {resetLink}");
        }

        public void ResetPassword(string token, string newPassword)
        {
            var email = _resetTokens.FirstOrDefault(x => x.Value.Token == token).Key;
            if (string.IsNullOrEmpty(email) || _resetTokens[email].Expiration < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Invalid or expired reset token.");
            }

            var user = _userRepository.GetByUsername(email);
            user.PasswordHash = HashPassword(newPassword);
            _userRepository.Update(user);
            Log.Information("Password reset successfully for {Email}", email);

            _resetTokens.Remove(email);
        }
        #endregion

        #region Private funtions
        private void TrackFailedLogin(string username)
        {
            if (!_loginAttempts.ContainsKey(username))
            {
                _loginAttempts[username] = 0;
            }

            _loginAttempts[username]++;

            if (_loginAttempts[username] >= MaxAttempts)
            {
                _lockoutEndTime[username] = DateTime.UtcNow.AddMinutes(LockoutDurationMinutes);
                _loginAttempts[username] = 0;
                Log.Warning("User {Username} locked out due to too many failed login attempts.", username);
            }
        }

        private void ResetLoginAttempts(string username)
        {
            if (_loginAttempts.ContainsKey(username))
            {
                _loginAttempts[username] = 0;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }
        private string GenerateResetToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        #endregion
    }
}
