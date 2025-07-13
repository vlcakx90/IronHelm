using Castle.Api.Knight;
using Castle.Interfaces;
using Castle.Models.Raven;
using Castle.Models.Knight;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Castle.Controllers
{
    [ApiController]
    [Route("soldier")]
    public class KnightController : ControllerBase
    {
        private readonly IKnightService _soldierService;
        private readonly IKnightHistoryService _soldierHistoryService;
        private readonly IRequestHeader _requestHeader;
        private readonly ILogger<KnightController> _logger;
        public KnightController(IKnightService soldierService, IRequestHeader requestHeader, IKnightHistoryService soldierHistoryService, ILogger<KnightController> logger)
        {
            _soldierService = soldierService;
            _requestHeader = requestHeader;
            _soldierHistoryService = soldierHistoryService;
            _logger = logger;
        }

        [HttpGet(Name = "GetKnights")]
        public async Task<ActionResult<IEnumerable<Knight>>> GetKnightss()
        {
            var soldiers = await _soldierService.GetKnights();

#if DEBUG
            _logger.LogInformation($"[+] GetKnights() > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(soldiers);
        }

        [HttpGet("{soldierId}", Name = "GetKnight")]
        public async Task<ActionResult<Knight>> GetKnight(string soldierId)
        {
            var soldier = await _soldierService.GetKnight(soldierId);
            if (soldier is null)
            {
#if DEBUG
                _logger.LogError($"[!] GetKnight({soldierId}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Knight not found");
#endif
                return NotFound();
            }

#if DEBUG
            _logger.LogInformation($"[+] GetKnight({soldierId}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(soldier);
        }

        [HttpGet("{soldierId}/tasks", Name = "GetTaskResults")] // To be replaced by Task History below --> (GetTaskHistorys)
        public ActionResult<IEnumerable<TaskResultMessage>> GetTaskResults(string soldierId)
        {
            //var agent = _soldierService.GetKnight(soldierId);
            //if (agent is null) return NotFound("Knight not found");

            //var results = agent.GetTaskResults();

            //var username = _requestHeader.GetUsername(Request.Headers.Authorization.ToString());
            //_logger.LogInformation($"[+] KnightController.GetTaskResults({soldierId}) > user: {username} @ {DateTime.UtcNow.ToLongTimeString()}");

            //return Ok(results);

            throw new NotImplementedException();
        }


        [HttpGet("{soldierId}/tasks/{taskId}", Name = "GetTaskResult")] // To be replaced by Task History below --> (GetTaskHistory)
        public async Task<ActionResult<TaskResultMessage>> GetTaskResult(string soldierId, string taskId)
        {
            //var agent = _soldierService.GetKnight(soldierId);
            //if (agent is null) return NotFound("Knight not found");

            //var result = agent.GetTaskResult(taskId);
            //if (result is null) return NotFound("Task not found");

            //var username = _requestHeader.GetUsername(Request.Headers.Authorization.ToString());
            //_logger.LogInformation($"[+] KnightController.GetTaskResult({soldierId} : {taskId}) > user: {username} @ {DateTime.UtcNow.ToLongTimeString()}");

            //return Ok(result);

            //throw new NotImplementedException();

            // using _soldierHistoryService
            KnightHistory hist = await _soldierHistoryService.Get(soldierId, taskId);
            TaskResultMessage result = new TaskResultMessage
            {
                TaskId = hist.TaskId,
                Result = hist.Result,
                CompletetedAt = hist.ResultsAt
            };

#if DEBUG
            _logger.LogInformation($"[+] GetTaskResult({soldierId}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(result);
        }

        //// AgentTaskHistory
        [HttpGet("{soldierId}/historys", Name = "GetKnightHistorys")] // Get agents List<AgentTaskHistory> _taskHistory by agent Id
        public async Task<ActionResult<IEnumerable<KnightHistory>>> GetKnightHistory(string soldierId)
        {
            var soldier = await _soldierService.GetKnight(soldierId);
            if (soldier is null)
            {
#if DEBUG
                _logger.LogError($"[!] GetKnightHistory({soldierId}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Knight not found");
#endif
                return NotFound("Knight not found");
            }

            //var results = agent.GetTaskHistorys();
            var histories = await _soldierHistoryService.GetAll(soldierId);

#if DEBUG
            _logger.LogInformation($"[+] GetKnightHistory({soldierId}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(histories);
        }

        // Might not be needed
        [HttpGet("{soldierId}/history/{taskId}", Name = "GetKnightHistory")] // Get agents single <AgentTaskHistory> from _taskHistory by agent & task Id
        public async Task<ActionResult<KnightHistory>> GetKnightHistory(string soldierId, string taskId)
        {
            var soldier = await _soldierService.GetKnight(soldierId);
            if (soldier is null)
            {
#if DEBUG
                _logger.LogError($"[!] GetKnightHistory({soldierId} : {taskId}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Knight not found");
#endif
                return NotFound("Knight not found");
            }

            //var history = agent.GetTaskHistory(taskId);
            var history = await _soldierHistoryService.Get(soldierId, taskId);
            if (history is null)
            {
#if DEBUG
                _logger.LogError($"[!] GetKnightHistory({soldierId} : {taskId}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Task not found");
#endif
                return NotFound("Task not found");
            }

#if DEBUG
            _logger.LogInformation($"[+] GetKnightHistory({soldierId} : {taskId}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok(history);
        }

        [HttpGet("{soldierId}/historyresult/{taskId}", Name = "GetHistoryResult")] // Get agents single <AgentTaskResult> _taskHistory.AgentTaskResult by agent & task Id
        public async Task<ActionResult<TaskResultMessage>> GetHistoryResult(string soldierId, string taskId)
        {
            //var agent = _soldierService.GetKnight(soldierId);
            //if (agent is null) return NotFound("Knight not found");

            //var history = agent.GetTaskHistoryResult(taskId);

            //if (history is null) return NotFound("Task not found");

            //var username = _requestHeader.GetUsername(Request.Headers.Authorization.ToString());
            //_logger.LogInformation($"[+] KnightController.GetHistoryResult({soldierId} : {taskId}) > user: {username} @ {DateTime.UtcNow.ToLongTimeString()}");

            //return Ok(history);

            throw new NotImplementedException();
        }
        //// AgentTaskHistory End

        [HttpPost("{soldierId}", Name = "TaskKnight")]
        public async Task<ActionResult<TaskMessage>> TaskKnight(string soldierId, [FromBody] TaskKnightRequest request)
        {
            var soldier = await _soldierService.GetKnight(soldierId);
            if (soldier is null)
            {
#if DEBUG
                _logger.LogError($"[!] TaskKnight({soldierId}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Knight not found");
#endif
                return NotFound();
            }

            //var task = new TaskMessage
            //{
            //    TaskId = Guid.NewGuid().ToString(),
            //    Command = request.Command,
            //    Arguments = request.Arguments,
            //    File = request.File,
            //    //StartTime = DateTime.Now, // now in KnightHistory
            //};

            // Add to agents list <AgentTaskHistory> _taskHistory
            //agent.QueueTask(task);
            var history = new KnightHistory(soldierId, request);
            await _soldierHistoryService.Add(history);

            var root = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.Path}";
            var path = $"{root}/tasks/{history.TaskId}";

#if DEBUG
            _logger.LogInformation($"[+] TaskKnight({soldierId}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Created(path, history);
        }

        [HttpDelete("{soldierId}", Name = "RemoveKnight")]
        public async Task<IActionResult> RemoveAgent(string soldierId)
        {
            Knight soldier = await _soldierService.GetKnight(soldierId);
            if (soldier is null)
            {
#if DEBUG
                _logger.LogError($"[!] RemoveKnight({soldierId}) > user: " +
                    $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                    $" @ {DateTime.UtcNow.ToLongTimeString()}" +
                    $"\n\t Knight not found");
#endif
                return NotFound();
            }

            _soldierService.RemoveKnight(soldier);

#if DEBUG
            _logger.LogInformation($"[+] RemoveKnight({soldierId}) > user: " +
                $"{_requestHeader.GetUsername(Request.Headers.Authorization.ToString())}" +
                $" @ {DateTime.UtcNow.ToLongTimeString()}");
#endif

            return Ok();
        }

    }
}
