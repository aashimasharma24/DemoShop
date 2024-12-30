using DemoShop.Core.DataObjects;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography;
using System.Text;

namespace DemoShop.Manager.Services
{
    public class AuthenticateUserService : IAuthenticateUserService
    {
        private readonly DemoShopDbContext _context;
        private readonly string _jwtKey;
        public AuthenticateUserService(DemoShopDbContext context, IConfiguration config)
        {
            _context = context;
            _jwtKey = config["Jwt:Key"];
            Log.Information("UserService initialized.");
        }

        public string Authenticate(string username, string password)
        {
            Log.Information("Authenticating user: {Username}", username);
            var user = _context.Users.FirstOrDefault(x => x.Username == username);
            if (user == null || user.PasswordHash != HashPassword(password) || String.IsNullOrWhiteSpace(_jwtKey))
            {
                Log.Warning("Authentication failed for user: {Username}", username);
                return null;
            }

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            Log.Information("Token generated for user: {Username}", username);
            return tokenHandler.WriteToken(token);
        }
        public void Register(User user)
        {
            Log.Information("Registering new user: {Username}", user.Username);
            user.PasswordHash = HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            _context.SaveChanges();
            Log.Information("User registered successfully: {Username}", user.Username);
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
