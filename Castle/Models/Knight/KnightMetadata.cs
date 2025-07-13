namespace Castle.Models.Knight
{
    public class KnightMetadata
    {
        public string Id { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
        public Integrity Integrity { get; set; }
        public string Username { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public int ProcessId { get; set; }
        public bool x64 { get; set; }
    }

    public enum Integrity
    {
        MEDIUM,
        HIGH,
        SYSTEM
    }
}
