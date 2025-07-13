//using Helm.Shared.SwaggerApi;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Helm.Shared.Services
//{
//    public class CastleService
//    {
//        private CastleApi client = null;
//        private string castleUrl = null; // not needed
//        //private string health = "1"; // Temp, will be null when below function is implemented

//        private async Task<HealthType> GetCastleHealth() // Something like this
//        {
//            HealthType health = (HealthType)await client.GetHealthStatusAsync();
//            return health;
//        }


//        ///////////////////////////// Public Access

//        // Constructor
//        public CastleService(string castleUrl)
//        {
//            this.castleUrl = castleUrl;

//            HttpClientHandler clientHandler = new HttpClientHandler
//            {
//                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
//            };

//            client = new CastleApi(castleUrl, new HttpClient(clientHandler));
//        }

//        public enum HealthType
//        {
//            Healthy,
//            UnHealthy,
//        }

//        public async Task<string> GetCastleUrlAync()
//        {
//            return castleUrl;
//        }

//        public async Task<HealthType> GetCastleHealthAync()
//        {
//            return (await GetCastleHealth()); // Something like this

//            //return health; // Just test for now
//        }
//    }
//}
