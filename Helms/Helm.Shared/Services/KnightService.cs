using Helm.Shared.SwaggerApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helm.Shared.Services
{
    public class KnightService
    {
        //////////////// Below was copied from AgentServiceOLD

        //static HttpClientHandler clientHandler = new HttpClientHandler
        //{
        //    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        //};
        //HelmApi client = new HelmApi("https://localhost:7014/", new HttpClient(clientHandler)); // HTTPS                
        //HelmApi client = new HelmApi("http://localhost:5000/", new HttpClient()); // HTTP

        private CastleApi _client = null;
        private string _castleUrl = null;

        public KnightService(string castleUrl)
        {
            this._castleUrl = castleUrl;

            HttpClientHandler clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true // Is not available in WASM
            };

            _client = new CastleApi(castleUrl, new HttpClient(clientHandler));
        }
        public async Task<List<Knight>> GetKnightsAsync()
        {
            ICollection<Knight> agents = await _client.GetKnightsAsync();
            return agents.ToList();
        }

        public async Task<List<KnightHistory>> GetKnightHistorysAsync(string agentId)
        {
            ICollection<KnightHistory> history = await _client.GetKnightHistorysAsync(agentId);
            return history.ToList();
        }

        public async Task<KnightHistory> GetKnightHistoryAsync(string agentId, string taskId) // Needed?
        {
            return await _client.GetKnightHistoryAsync(agentId, taskId);
        }

        public async Task<TaskResultMessage> GetTaskResultAsync(string agentId, string taskId)
        {
            return await _client.GetTaskResultAsync(agentId, taskId);
        }

        public async Task<TaskMessage> TaskKnightAsync(string id, TaskKnightRequest req)
        {
            return await _client.TaskKnightAsync(id, req);
        }

        public async Task RemoveKnightAsync(string id)
        {
            await _client.RemoveKnightAsync(id);
        }
    }
}










//-------------------------------------------- Original 
//using Helm.Shared.SwaggerApi;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Helm.Shared.Services
//{
//    public class KnightService
//    {
//        //////////////// Below was copied from AgentServiceOLD

//        //static HttpClientHandler clientHandler = new HttpClientHandler
//        //{
//        //    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
//        //};
//        //HelmApi client = new HelmApi("https://localhost:7014/", new HttpClient(clientHandler)); // HTTPS                
//        //HelmApi client = new HelmApi("http://localhost:5000/", new HttpClient()); // HTTP

//        private CastleApiOLD _client = null;
//        private string _castleUrl = null; // Not Needed

//        public KnightService(string castleUrl)
//        {
//            this._castleUrl = castleUrl;

//            HttpClientHandler clientHandler = new HttpClientHandler
//            {
//                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
//            };

//            _client = new CastleApiOLD(castleUrl, new HttpClient(clientHandler));
//        }
//        public async Task<List<Agent>> GetAgentsAsync()
//        {
//            ICollection<Agent> agents = await _client.GetAgentsAsync();
//            return agents.ToList();
//        }

//        public async Task<List<AgentTaskHistory>> GetAgentHistoryAsync(string agentId)
//        {
//            ICollection<AgentTaskHistory> history = await _client.GetTaskHistorysByAgentIdAsync(agentId);
//            return history.ToList();
//        }

//        public async Task<AgentTaskHistory> GetSingleAgentHistoryAsync(string agentId, string taskId) // Needed?
//        {
//            return await _client.GetTaskHistoryByBothIdAsync(agentId, taskId);
//        }

//        public async Task<AgentTaskResult> GetTaskResultAsync(string agentId, string taskId)
//        {
//            return await _client.GetTaskHistoryResultByBothIdAsync(agentId, taskId);
//        }

//        public async Task<AgentTask> TaskAgentAsync(string id, TaskAgentRequest req)
//        {
//            return await _client.TaskAgentAsync(id, req);
//        }

//        public async Task RemoveAgentAsync(string id)
//        {
//            await _client.RemoveAgentAsync(id);
//        }
//    }
//}
