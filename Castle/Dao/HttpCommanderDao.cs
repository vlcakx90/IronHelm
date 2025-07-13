using SQLite;
using Castle.Models.Commander;

namespace Castle.Dao
{
    [Table("commanders")]
    public sealed class HttpCommanderDao
    {
        //[PrimaryKey, AutoIncrement]   // Can auto increment like this, but will probably generate my own id's
        //public int Id { get; set; }

        [PrimaryKey, Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("Type")]
        public COMMANDER_TYPE Type { get; set; }
        //[Column("Secure")]
        //public bool Secure { get; set; }

        [Column("BindPort")]
        public int BindPort { get; set; }

        //[Column("Passwd")]
        //public string Passwd { get; set; } = string.Empty;

        [Column("Tls")]
        public bool Tls {  get; set; }

        // type -> could be changed to a bool (secure or not)
        // name 
        // bindport

        public static implicit operator HttpCommanderDao(Commander listener)
        {
            return new HttpCommanderDao
            {
                Name = listener.Name,
                Type = listener.Type,                
                BindPort = listener.BindPort,
                //Passwd = listener.Passwd,
                Tls = listener.Tls,
            };
        }

        public static implicit operator HttpCommander(HttpCommanderDao dao)
        {
            return new HttpCommander(dao.Name, dao.BindPort, dao.Tls);
        }
    }
}
