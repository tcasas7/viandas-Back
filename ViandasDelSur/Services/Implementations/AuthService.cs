using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models;
using ViandasDelSur.Services.Interfaces;
using ViandasDelSur.Tools;
using ViandasDelSur.Models.Responses;

namespace ViandasDelSur.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly Encrypter _encrypter;
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _encrypter = new Encrypter();
            _configuration = configuration;
        }

        public Response Login(LoginDTO model, User user)
        {
            Response response = new Response();

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Credenciales invalidas";
                return response;
            }

            if (!(_encrypter.ValidateText(model.password, user.hash, user.salt)))
            {
                response.statusCode = 401;
                response.message = "Contraseña incorrecta";
                return response;
            }

            response.statusCode = 200;
            response.message = "Ok";
            return response;
        }

        public string MakeToken(string email, string role, int minutes)
        {
            var claims = new[]
                {
                    new Claim("Account", email),
                    new Claim("Role", role)
                };    

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Key").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            int expirationMinutes = int.Parse(_configuration["JWT:TokenExpirationMinutes"]);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
