namespace SimplzKeyGenVerifier.Options
{
    public class JwtOptions
    {
        public int TokenExpirationMonths { get; set; } = 12;
        public bool IsEnabled { get; set; } = false;
    }
}
