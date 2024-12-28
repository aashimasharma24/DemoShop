using DemoShop.Manager.DBContext;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
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
            if (user == null || user.PasswordHash != password || String.IsNullOrWhiteSpace(_jwtKey))
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
    }
}
