namespace SimplzKeyGenVerifier.Services
{
    public interface IJwtHandler
    {
        public Dictionary<string, object> ReadToken(string token, string licenceCode);
        public string WriteToken(Dictionary<string, object> pl);
        public string WriteToken(Dictionary<string, object> pl, string presharedKey);
    }
}
