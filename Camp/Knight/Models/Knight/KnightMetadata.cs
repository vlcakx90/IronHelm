namespace Knight.Models.Knight
{
    public class KnightMetadata
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string Hostname { get; set; }
        public Integrity Integrity { get; set; }
        public string Username { get; set; }
        public string ProcessName { get; set; }
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