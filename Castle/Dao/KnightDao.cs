using Castle.Models.Raven;
using Castle.Models.Knight;
using SQLite;

namespace Castle.Dao
{
    [Table("knight")]
    public class KnightDao // Metadata
    {
        [PrimaryKey, Column("id")]
        public string Id { get; set; } = string.Empty;

        [Column("address")]
        public string Address { get; set; } = string.Empty;

        [Column("hostname")]
        public string Hostname { get; set; } = string.Empty;

        [Column("integrity")]
        public Integrity Integrity { get; set; }

        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Column("processname")]
        public string ProcessName { get; set; } = string.Empty;

        [Column("process_id")]
        public int ProcessId { get; set; }

        [Column("x64")]
        public bool x64 { get; set; }

        [Column("first_seen")]
        public DateTime FirstSeen { get; set; }

        [Column("last_seen")]
        public DateTime LastSeen { get; set; }




        [Column("parent_id")]
        public string ParentId { get; set; } = string.Empty;

        [Column("direction")]
        public AllyDirection Direction { get; set; }

        public static implicit operator KnightDao(Knight knight)
        {
            return new KnightDao
            {
                Id = knight.Metadata.Id,
                Address = knight.Metadata.Address,
                Hostname = knight.Metadata.Hostname,
                Integrity = knight.Metadata.Integrity,
                Username = knight.Metadata.Username,
                ProcessName = knight.Metadata.ProcessName,
                ProcessId = knight.Metadata.ProcessId,
                x64 = knight.Metadata.x64,
                FirstSeen = knight.FirstSeen,
                LastSeen = knight.LastSeen,

                ParentId = knight.ParentId,
                Direction = knight.Direction,
            };
        }

        public static implicit operator Knight(KnightDao dao)
        {
            return dao is null
                ? null
                : new Knight
            {                
                Metadata = new KnightMetadata
                {
                    Id = dao.Id,
                    Address = dao.Address,
                    Hostname = dao.Hostname,
                    Integrity = dao.Integrity,
                    Username = dao.Username,
                    ProcessName = dao.ProcessName,
                    ProcessId = dao.ProcessId,
                    x64 = dao.x64,                    
                },

                FirstSeen = dao.FirstSeen,
                LastSeen = dao.LastSeen,
            };
        }
    }
}
