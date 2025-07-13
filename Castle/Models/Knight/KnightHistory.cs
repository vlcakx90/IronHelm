using Castle.Api.Knight;
using Castle.Models.Raven;

namespace Castle.Models.Knight
{
    public class KnightHistory
    {
        //public DateTime TaskCreatedAt { get; set; }
        //public SOLDIER_TASK_STATUS Status { get; set; }

        //public string SoldierId { get; set; }
        //public TaskMessage SoldierTask { get; set; } = new TaskMessage();
        //public TaskResultMessage SoldierTaskResult { get; set; } = new TaskResultMessage();



        // Task
        public string TaskId { get; set; }
        public string KnightId { get; set; }
        public string Command { get; set; }
        public string[] Arguments { get; set; }
        public byte[] File { get; set; }

        public DateTime TaskSentAt { get; set; }
        public KNIGHT_TASK_STATUS Status { get; set; }

        // Result
        public string Result { get; set; }
        public DateTime ResultsAt { get; set; }

        public KnightHistory()
        {
            
        }
        public KnightHistory(string knightId, TaskMessage message)
        {
            KnightId = knightId;

            TaskId = message.TaskId;
            Command = message.Command;
            Arguments = message.Arguments;
            File = message.File;

            TaskSentAt = DateTime.UtcNow;
            Status = KNIGHT_TASK_STATUS.PENDING;

            Result = "Pending";
            //CompletetedAt = 
        }

        public KnightHistory(string knightId, TaskKnightRequest request)
        {
            KnightId = knightId;

            TaskId = Guid.NewGuid().ToString();
            Command = request.Command;
            Arguments = request.Arguments;
            File = request.File;

            TaskSentAt = DateTime.UtcNow;
            Status = KNIGHT_TASK_STATUS.PENDING;

            Result = "Pending";
            //CompletetedAt = 

        }

        public void Update(TaskResultMessage resultMsg)
        {
            Status = KNIGHT_TASK_STATUS.FINISHED;

            Result = resultMsg.Result;
            ResultsAt = resultMsg.CompletetedAt;
        }

    }

    public enum KNIGHT_TASK_STATUS
    {
        //ERROR,
        PENDING, // sitting in db waiting for soldier to get
        TASKED,  // soldier got task
        FINISHED // soldier posted results back
    }
}
