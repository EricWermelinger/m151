using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using m151_backend.DTOs;
using Microsoft.IdentityModel.Tokens;

namespace m151_backend.Framework
{
    public class AuthorizationM151
    {
        private const string TOKEN = "9D31AC86-3FA6-49CB-9226-564BF3D41AA9";
        private const int VALID_TIME = 3;
        
        // todo refresh Controller

        public TokenDTO CreateToken(Guid userId)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userId.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(TOKEN));

            var credits = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddMinutes(VALID_TIME),
                signingCredentials: credits);

            return new TokenDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}
