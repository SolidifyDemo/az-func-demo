using System.Net;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;

namespace WorkflowPayload.Function
{
    public class SaveWorkflowPayload
    {
        private readonly ILogger _logger;

        public SaveWorkflowPayload(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SaveWorkflowPayload>();
        }

        [Function("SaveWorkflowPayload")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try {

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Payload saved");

            // get payload from request body and save to azure stroage
            var payload = await req.ReadAsStringAsync();
            _logger.LogInformation($"Payload: {payload}");

            if (payload != null)
            {
                // find header with key "X-GitHub-Event" and check if value equal to "workflow_job"
                var header = req.Headers.FirstOrDefault(h => h.Key == "X-GitHub-Event");
                if (header.Value != null && string.Join(",", header.Value) != "workflow_job")
                {
                    _logger.LogWarning("Header X-GitHub-Event is not equal to workflow_job");
                    response.WriteString("Not a workflow_job event");
                    return response;
                }
                
               // check if body contains key action equal to "completed"
                var body = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(payload);
                if (body != null && body.ContainsKey("action") && string.Join(",", body["action"]) != "completed")
                {
                    _logger.LogWarning("Body action is not equal to completed");
                    response.WriteString("Not a workflow completed action");
                    return response;
                }

                // save payload to Azure Blob Storage container

                var connectionString = "<Connection-String>";
                var containerName = "demo-blob";
                var blobName = Guid.NewGuid().ToString();
                var blobClient = new BlobClient(connectionString, containerName, blobName);
                await blobClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes(payload)));
         
            }
            else
            {
                _logger.LogWarning("Payload is null or empty.");
            }

            return response;
             }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving payload");
            }

            return null;
        }
    }
}
