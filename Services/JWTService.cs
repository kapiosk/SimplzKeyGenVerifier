using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimplzKeyGenVerifier.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SimplzKeyGenVerifier.Services
{
    internal class JwtService : IJwtHandler
    {
        private readonly int _tokenExpirationMonths;
        public JwtService(IOptions<JwtOptions> options)
        {
            _tokenExpirationMonths = options.Value.TokenExpirationMonths;
        }

        public Dictionary<string, object> ReadToken(string token, string presharedKey)
        {
            JwtSecurityTokenHandler handler = new();
            SymmetricSecurityKey SSKIn = new(Encoding.UTF8.GetBytes(presharedKey));

            try
            {
                handler.ValidateToken(token, new()
                {
                    IssuerSigningKey = SSKIn,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out var tk);
                if (tk is JwtSecurityToken jwt) return jwt.Payload;
                else throw new SecurityTokenValidationException();
            }
            catch
            {
                throw new SecurityTokenValidationException();
            }
        }

        public string WriteToken(Dictionary<string, object> pl,string presharedKey)
        {
            JwtSecurityTokenHandler handler = new();
            SymmetricSecurityKey SSKOut = new(Encoding.UTF8.GetBytes(presharedKey));
            SigningCredentials signingCredentials = new(SSKOut, SecurityAlgorithms.HmacSha256);
            var token = handler.CreateJwtSecurityToken(signingCredentials: signingCredentials, expires: DateTime.UtcNow.AddMonths(_tokenExpirationMonths));
            foreach (var p in pl)
                token.Payload.Add(p.Key, p.Value);
            return handler.WriteToken(token);
        }

        public string WriteToken(Dictionary<string, object> pl)
        {
            throw new NotImplementedException();
        }
    }
}
