using Helm.Shared.SwaggerApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helm.Shared.Services
{
    public class CommanderService
    {
        //static HttpClientHandler clientHandler = new HttpClientHandler
        //{
        //    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        //};
        //HelmApi client = new HelmApi("https://localhost:7014/", new HttpClient(clientHandler)); // HTTPS                
        //HelmApi client = new HelmApi("http://localhost:5000/", new HttpClient()); // HTTP

        private CastleApi client = null;
        private string castleUrl = null;
        public enum ListenerType
        {
            HTTP,
            //HTTPS,
            //DNS,
        }
        public CommanderService(string castleUrl)
        {
            this.castleUrl = castleUrl;

            HttpClientHandler clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };

            client = new CastleApi(castleUrl, new HttpClient(clientHandler));
        }
        public async Task<List<Commander>> GetCommandersAsync()
        {
            ICollection<Commander> commanders = await client.GetCommandersAsync();
            return commanders.ToList();
        }

        public async Task<Commander> CreateCommanderAsync(StartHttpCommanderRequest slr)
        {
            Commander commander = await client.StartCommanderAsync(slr);
            return commander;
        }

        public async Task DeleteCommanderAsync(string name)
        {
            await client.StopCommanderAsync(name);
            return;
        }

        //public async Task<string> TestConstr()
        //{
        //    return castleUrl;
        //}

        //public async Task<List<Commander>> GetListenersAsync()
        //{
        //    return (await GetCommanders());
        //}

        //public async Task<Commander> CreateListenerAsync(StartHttpCommanderRequest slr)
        //{
        //    return (await CreateCommander(slr));
        //}

        //public async Task DeleteListenerAsync(string name)
        //{
        //    await DeleteCommander(name);
        //    return;
        //}
    }
}
