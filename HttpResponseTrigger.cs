
using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace DemoProjects
{
    public class HttpResponseTrigger
    {
        private readonly ILogger<HttpResponseTrigger> _logger;

        public HttpResponseTrigger(ILogger<HttpResponseTrigger> logger)
        {
            _logger = logger;
        }

        [Function("HttpResponseTrigger")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
            [DurableClient] DurableTaskClient starter
        )
        {
            // Handling exceptions in Azure functions is critical to retries working correctly.
            try
            {
                var input = await req.ReadAsStringAsync();
                DemoDto dto = JsonSerializer.Deserialize<DemoDto>(input);
                // Do work: put message on queue, start orchestration
            }
            catch (JsonException)
            {
                return req.CreateResponse(HttpStatusCode.UnprocessableEntity);
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
