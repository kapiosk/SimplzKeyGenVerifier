using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimplzKeyGenVerifier.Data;
using SimplzKeyGenVerifier.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace SimplzKeyGenVerifier.Services
{
    public class JwtAsymmetricService : IJwtHandler
    {
        private readonly int _tokenExpirationMonths;
        private readonly byte[] _privateKey;
        private readonly byte[] _publicKey;

        public JwtAsymmetricService(IOptions<JwtAsymmetricOptions> options)
        {
            _tokenExpirationMonths = options.Value.TokenExpirationMonths;
            _privateKey = Convert.FromBase64String(options.Value.PrivateKey);
            _publicKey = Convert.FromBase64String(options.Value.PublicKey);
        }

        public Dictionary<string, object> ReadToken(string token, string publicKey)
        {
            JwtSecurityTokenHandler handler = new();
            using RSA rsa = RSA.Create(2048);
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);

            try
            {
                handler.ValidateToken(token, new()
                {
                    IssuerSigningKey = new RsaSecurityKey(rsa),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    CryptoProviderFactory = new CryptoProviderFactory()
                    {
                        CacheSignatureProviders = false
                    }
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
            using RSA rsa = RSA.Create(2048);
            rsa.ImportRSAPrivateKey(_privateKey, out _);

            var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };
            var token = handler.CreateJwtSecurityToken(signingCredentials: signingCredentials, expires: DateTime.UtcNow.AddMonths(_tokenExpirationMonths));
            foreach (var p in pl)
                token.Payload.Add(p.Key, p.Value);
            return handler.WriteToken(token);
        }
    }
}
