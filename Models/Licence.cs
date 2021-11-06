namespace SimplzKeyGenVerifier.Models
{
    public record Licence
    {
        public int LicenceId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid LicenceCode { get; set; } = Guid.NewGuid();
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public List<Log> Logs { get; set; } = new();
        public string? Hash { get; set; }
    }
}
