namespace SimplzKeyGenVerifier.Options
{
    public class JwtAsymmetricOptions
    {
        public int TokenExpirationMonths { get; set; } = 5;
        public string PublicKey { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
    }
}
