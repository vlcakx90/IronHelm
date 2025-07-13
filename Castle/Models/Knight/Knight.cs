
using Castle.Models.Raven;

namespace Castle.Models.Knight
{
    public class Knight
    {
        public KnightMetadata Metadata { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }


        //// I am choosing not to have this in the Metadata so it will only be sent when a SoldierP2P checks in and saved here
        public string ParentId { get; set; }
        public AllyDirection Direction { get; set; }
        //// end
        

        // Leave these next three lines (containers) ... will change to use Tasks & Results tables with a History Join table to link to a Soldier
        //private readonly ConcurrentQueue<TaskMessage> _pendingTasks = new();
        //private readonly List<TaskResultMessage> _taskResults = new();
        //private readonly List<SoldierHistory> _history = new();       // I think I still need the above Queue, but idk about the List<AgentTaskResult>

        public Knight()
        {
            
        }

        public Knight(KnightMetadata metadata)
        {
            Metadata = metadata;
            FirstSeen = DateTime.UtcNow;
        }

        public Knight(KnightMetadata metadata, string parentId, AllyDirection direction)
        {
            Metadata = metadata;
            FirstSeen = DateTime.UtcNow;

            ParentId = parentId;
            Direction = direction;
        }

        public void CheckIn()
        {
            LastSeen = DateTime.UtcNow;
        }

        //public void QueueTask(TaskMessage task)       // NOW HANDLED BY RAVENSERVICE + SOLDIERHISTORYSERVICE
        //{
        //    _pendingTasks.Enqueue(task);

        //    //// AgentTaskHistory
        //    Console.WriteLine($"[+] Task queued: {task.TaskId}"); // DEBUG
        //    //_history.Add(new SoldierHistory
        //    //{
        //    //    TaskCreatedAt = DateTime.UtcNow,
        //    //    Status = SOLDIER_TASK_STATUS.PENDING,
        //    //    SoldierTask = task,
        //    //    SoldierTaskResult = new TaskResultMessage // Could be null instead
        //    //    {
        //    //        //Id = "Waiting",
        //    //        Id = task.Id,
        //    //        Result = "Pending",
        //    //        CompletetedAt = new DateTime(2024, 1, 2, 0, 0, 0) // Just a random date
        //    //    },                
        //    //});
        //    _history.Add(new SoldierHistory(Metadata.Id, task));
        //    //// AgentTaskHistory End
        //}

        //public IEnumerable<TaskMessage> GetPendingTasks()
        //{
        //    List<TaskMessage> tasks = new();

        //    while (_pendingTasks.TryDequeue(out var task))
        //    {
        //        tasks.Add(task);
        //    }

        //    return tasks;
        //}

        //public TaskResultMessage GetTaskResult(string taskId)
        //{
        //    return GetTaskResults().FirstOrDefault(r => r.TaskId.Equals(taskId));
        //}

        //public IEnumerable<TaskResultMessage> GetTaskResults()
        //{
        //    return _taskResults;
        //}


        ////// AgentTaskHistory
        //public TaskResultMessage GetTaskHistoryResult(string taskId)
        //{
        //    //AgentTaskResult result = GetTaskHistorys().FirstOrDefault(r => r.AgentTaskResult.Id.Equals(taskId)).AgentTaskResult;
        //    //if (result is not null)
        //    //{
        //    //    return result;
        //    //}
        //    return GetTaskHistorys().FirstOrDefault(r => r.SoldierTaskResult.Id.Equals(taskId)).SoldierTaskResult;
        //}
        //public SoldierHistory GetTaskHistory(string taskId)
        //{
        //    return GetTaskHistorys().FirstOrDefault(r => r.SoldierTaskResult.Id.Equals(taskId));
        //}
        //public IEnumerable<SoldierHistory> GetTaskHistorys()
        //{
        //    return _history;
        //}
        ////// AgentTaskHistory End

        //public void AddTaskResults(IEnumerable<TaskResultMessage> results)
        //{
        //    // Add to agents list <AgentTaskHistory> _taskHistory         .......... idk what this was for oops
        //    _taskResults.AddRange(results);

        //    //// AgentTaskHistory
        //    foreach (TaskResultMessage result in results)
        //    {
        //        SoldierHistory taskHistory = _history.FirstOrDefault(id => id.SoldierTask.Id.Equals(result.TaskId));
        //        if (taskHistory != null && taskHistory.Status == SOLDIER_TASK_STATUS.PENDING) // To stop overwriting existing result? shouldnt happen though I think rn
        //        {
        //            Console.WriteLine($"[*] Result queued: {result.TaskId}");
        //            taskHistory.SoldierTaskResult = result;
        //            taskHistory.Status = SOLDIER_TASK_STATUS.FINISHED;
        //        }
        //    }
        //    //// AgentTaskHistory End
        //}
    }
}
