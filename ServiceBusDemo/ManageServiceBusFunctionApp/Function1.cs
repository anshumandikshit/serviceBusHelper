using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ServiceBusHelper;
using Newtonsoft.Json;
using ServiceBusHelper.DTO;

namespace ManageServiceBusFunctionApp
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string queueName = req.Query["queueName"];

            var queueDescription = new QueueDescriptorDTO()
            {
                EnableDeadLetteringOnMessageExpiration = true,
                MaxDeliveryCount = 20,
                RequiresDuplicateDetection = true,
                RequiresSession = true

            };
            var createQueueStatus = await ServiceBusHelper.ServiceBusHelper.CreateQueueAsync(queueName,queueDescription);

            string responseMessage = createQueueStatus;

            return new OkObjectResult(responseMessage);
        }
    }
}
