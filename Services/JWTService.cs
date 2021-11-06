using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimplzKeyGenVerifier.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SimplzKeyGenVerifier.Services
{
    internal class JWTService
    {
        public string _tokenSecret;
        public int _tokenExpirationMinutes;
        public JWTService(IOptions<JWTOptions> options)
        {
            _tokenSecret = "";
            _tokenExpirationMinutes = options.Value.TokenExpirationMinutes;
        }

        public static Dictionary<string, object> ReadToken(string token, string issuerSigningKey)
        {
            JwtSecurityTokenHandler handler = new();
            SymmetricSecurityKey SSKIn = new(Encoding.UTF8.GetBytes(issuerSigningKey));
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

        public string WriteToken(Dictionary<string, object> pl)
        {
            JwtSecurityTokenHandler handler = new();
            SymmetricSecurityKey SSKOut = new(Encoding.UTF8.GetBytes(_tokenSecret));
            SigningCredentials signingCredentials = new(SSKOut, SecurityAlgorithms.HmacSha256);
            var token = handler.CreateJwtSecurityToken(signingCredentials: signingCredentials, expires: DateTime.UtcNow.AddMinutes(_tokenExpirationMinutes));
            foreach (var p in pl)
                token.Payload.Add(p.Key, p.Value);
            return handler.WriteToken(token);
        }
    }
}
