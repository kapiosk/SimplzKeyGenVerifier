namespace SimplzKeyGenVerifier.Services
{
    public interface IJwtHandler
    {
        public Dictionary<string, object> ReadToken(string token, string issuerSigningKey);
        public string WriteToken(Dictionary<string, object> pl);
    }
}
