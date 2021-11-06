using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace SimplzKeyGenVerifier.Services
{
    public class JwtAsymmetricService : IJwtHandler
    {
        private readonly int _tokenExpirationMonths = 5;
        private readonly string privateKey = @"MIIEpQIBAAKCAQEA2DLJWD9/JCn7CbkJ25/bRrK66VJveR9ogNha/siP1yMHqni9
Tp1FG0hSpyWVH/IUF220jN70+D6rRR6a8Czt5+1maZEzYvjAfqTi7vwuUGyciDe/
fnfhVaL6aba1Uxs01tVtabXqiSl853w5Prmbsgh3CIK9S6WxLseJKVOedX6z9n79
rwEZMtI7xfyLOODyKLyD+HvDRrIEos2qwn1/x1NUpw5tuUERXAvnkOTFr98A81E3
EDXE/ZSWibnIjA9x4mb0pUF/p3uuwWcy67jZsyN2Yz0hqzE7lXmnmEqs7dm7iTjy
Wm/iW/8zvBJf70AyIVBA3RLuLITeeMrIk+jEbQIDAQABAoIBAHWZKmwWHdJOA38G
Z5ZPHbcXARRpArmfm/h5twVfkt4y77iKdG6OnUh5znuctcDqrIMR6WO24wklDYRL
YXvC9KVzPIGvuYPcStiYV9gm1AprCboOmNrx2l/6hrt55qeN5O/2p80YZrq1FnvT
1k+IJmhwsk6uzsiXGAmZ/8G6+jZt9LU29653VtIkNaBqG+q7dpD327GCll1/bT4h
HikCh/8H18FwF7EKkQhtv3IRS5nnJClCu0oSC6PEULcZRoA8sjKX3D0Lv2SW3rST
ixp/1o8gFwxy4Gkfozps4P+AI+QPCSgBptNkScWnevwS2CxkL+1X6mc/xxbOEkaJ
572iYwECgYEA90EVFVoUQv9emLtGM/AeiO95lTEMSINPs3Wr4ElIG2kqD9wnGMVd
Sfpytkk839bPNTTgD5a3o5bHlDwJ6UJttgJhIWUdtamax265siiedvoqmeO+X93D
a0GDFQlFlo48sCdLyIISnp8id77KiHZ+u9cdN4bEsWeJlFdviHvrS1sCgYEA39h9
WSAxR6fEeLmYCIrGTu/Z0nIw6q0qoLScwwwMIqIRqkx0tbEAQmtCRUYoJPaSVm1F
sFhOLPcAkUYIIjTtNN8IBEc00erf9fK4E4FNDKtASRrFNsz4EPHznjCQF/OzPYs2
l//Pfy9o6MZ9Aq4YlWrGZi7vr4Iv2WEKBwxtYdcCgYEA0no/pV56MxJKh7+cImh8
xdr96AfJygymE0HPX21iiZr7aGbjjU2tfWVKs2wi+vVUjJmHRx3pE8Qt/z0gvsWC
uVbLMm9uXA+WJ8FVKJk1VFjfXQMnM1qHmU06dfTfnStV9Cpvc69uhEM5RzBOHt6o
QeT8BSGfXNmjn3jTBB9hDFMCgYEA2nBXEixaKsTK88yZUmyqTMm+1KDen4RQ/vaB
q80L4pgpiPiHQ+7wg+BLkXJ/JmMANxr3ULk7wKFhimK++TKQ8HFdT191agw8dbRg
cZhSajSLzUs0fN/UjCSwSgS1+Mm2di5uHjJieIj5ZX6c64qdLuI9reXLvIfczdLm
iC5WZ40CgYEAyD4+CLZaD6Q4vZzTQXDHl8UQxr02onTmaIYwIfzKCjMoqIEZI1YE
Pa5YNQ6u6Y1YJ0C3sfmJ/C08V5TqC50jOV1VhxN310Qg01nfTtCp8QqCHLARkiI1
35Cktwr39ncTG6+hGPag9G/OfIKpwPf9fI+PjFQFmxQjZfLlLeOrE2Y=";

        private readonly string publicKey = @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA2DLJWD9/JCn7CbkJ25/b
RrK66VJveR9ogNha/siP1yMHqni9Tp1FG0hSpyWVH/IUF220jN70+D6rRR6a8Czt
5+1maZEzYvjAfqTi7vwuUGyciDe/fnfhVaL6aba1Uxs01tVtabXqiSl853w5Prmb
sgh3CIK9S6WxLseJKVOedX6z9n79rwEZMtI7xfyLOODyKLyD+HvDRrIEos2qwn1/
x1NUpw5tuUERXAvnkOTFr98A81E3EDXE/ZSWibnIjA9x4mb0pUF/p3uuwWcy67jZ
syN2Yz0hqzE7lXmnmEqs7dm7iTjyWm/iW/8zvBJf70AyIVBA3RLuLITeeMrIk+jE
bQIDAQAB";

        public JwtAsymmetricService()
        {

        }

        public Dictionary<string, object> ReadToken(string token, string issuerSigningKey)
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
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);

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
