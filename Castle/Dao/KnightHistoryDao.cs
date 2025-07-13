using System.Runtime.Serialization;
using System.Text;
using SQLite;
using Castle.Models.Knight;

namespace Castle.Dao
{
    [Table("knight_history")]
    public class KnightHistoryDao
    {
        // Task
        [PrimaryKey, Column("task_id")]        
        public string TaskId { get; set; } = string.Empty;

        [Column("knight_id")]
        public string KnightId { get; set; } = string.Empty;

        [Column("command")]
        public string Command { get; set; } = string.Empty;

        [Column("arguments")]
        //public string[] Arguments { get; set; } = new string[0];
        public string Arguments { get; set; } = string.Empty;

        [Column("file")]
        public byte[] File { get; set; } = new byte[0];



        [Column("task_created_at")]
        public DateTime TaskCreatedAt { get; set; }

        [Column("status")]
        public KNIGHT_TASK_STATUS Status { get; set; }



        // Task Result
        //[Column("id")]
        //public string Id { get; set; }

        [Column("result")]
        public string Result { get; set; } = string.Empty;

        [Column("completed_at")]
        public DateTime CompletetedAt { get; set; }




        public static implicit operator KnightHistoryDao(KnightHistory knightHistory)
        {
            StringBuilder args = new StringBuilder();
            foreach (var arg in knightHistory.Arguments)
            {
                if (arg != string.Empty)
                {
                    args.Append(arg);
                    args.Append(' ');
                }
            }
            var arguments = args.ToString();
            if (args.Length > 0)
            {
                arguments.Remove(args.Length - 1, 1);
            }

            return new KnightHistoryDao
            {
                KnightId = knightHistory.KnightId,
                TaskId = knightHistory.TaskId,
                Command = knightHistory.Command,
                Arguments = arguments,
                File = knightHistory.File,

                TaskCreatedAt = knightHistory.TaskSentAt,
                Status = knightHistory.Status,

                Result = knightHistory.Result,
                CompletetedAt = knightHistory.ResultsAt,
            };
        }

        public static implicit operator KnightHistory(KnightHistoryDao dao)
        {
            return new KnightHistory
            {
                TaskSentAt = dao.TaskCreatedAt,
                Status = dao.Status,

                KnightId = dao.KnightId,
                TaskId = dao.TaskId,
                Command = dao.Command,
                Arguments = dao.Arguments.Split(' '),
                File = dao.File,

                Result = dao.Result,
                ResultsAt = dao.CompletetedAt

            };
        }
    }
}
