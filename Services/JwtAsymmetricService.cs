using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SimplzKeyGenVerifier.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace SimplzKeyGenVerifier.Services
{
    public class JwtAsymmetricService : IJwtHandler
    {
        private readonly int _tokenExpirationMonths;
        private readonly byte[] _privateKey;
        private const string pKey = @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2DLJWD9/JCn7CbkJ25/b RrK66VJveR9ogNha/siP1yMHqni9Tp1FG0hSpyWVH/IUF220jN70+D6rRR6a8Czt 5+1maZEzYvjAfqTi7vwuUGyciDe/fnfhVaL6aba1Uxs01tVtabXqiSl853w5Prmb sgh3CIK9S6WxLseJKVOedX6z9n79rwEZMtI7xfyLOODyKLyD+HvDRrIEos2qwn1/ x1NUpw5tuUERXAvnkOTFr98A81E3EDXE/ZSWibnIjA9x4mb0pUF/p3uuwWcy67jZ syN2Yz0hqzE7lXmnmEqs7dm7iTjyWm/iW/8zvBJf70AyIVBA3RLuLITeeMrIk+jE bQIDAQAB";

        public JwtAsymmetricService(IOptions<JwtAsymmetricOptions> options)
        {
            _tokenExpirationMonths = options.Value.TokenExpirationMonths;
            _privateKey = Convert.FromBase64String(options.Value.PrivateKey);
            //_privateKey = Convert.FromBase64String(key);
        }

        public Dictionary<string, object> ReadToken(string token, string publicKey)
        {
            JwtSecurityTokenHandler handler = new();
            using RSA rsa = RSA.Create(2048);
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(pKey), out _);

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
