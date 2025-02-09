using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using Xenhey.BPM.Core.Net8.Implementation;
using Xenhey.BPM.Core.Net8;
using Microsoft.Azure.Functions.Worker;


namespace documentmanagement
{
    public class snapshot
    {
        private readonly ILogger _logger;

        public snapshot(ILogger<snapshot> logger)
        {
            _logger = logger;
        }

        private HttpRequest _req;
        private NameValueCollection nvc = new NameValueCollection();
        [Function("snapshot")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post","get", Route = null)]
            HttpRequest req)
        {
            _req = req;

            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(_req.Body).ReadToEndAsync();
            _req.Headers.ToList().ForEach(item => { nvc.Add(item.Key, item.Value.FirstOrDefault()); });
            var results = orchrestatorService.ReturnFile(requestBody);
            return resultSet(results);

        }

 private ActionResult resultSet(byte[] reponsePayload)
        {
            var mediaSelectedtype = nvc.Get("Content-Type");
            var returnContent = new FileContentResult(reponsePayload, mediaSelectedtype);
            return returnContent;
        }
        private IOrchestrationService orchrestatorService
        {
            get
            {
                return new RemoteOrchrestratorService(nvc);
            }
        }
   }
}
