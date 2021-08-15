using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static ServiceBusHelper.Enums.ServiceBusEnums;
using ServiceBusHelper.DTO;
using System.Collections.Generic;
using System.Linq;

namespace ManageServiceBusFunctionApp
{
    public static class SendMessage
    {
        [FunctionName("SendMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string queueName = req.Query["queueName"];
            string topicName = req.Query["topicName"];
            string requestJSON = await req.ReadAsStringAsync();
            var messages = JsonConvert.DeserializeObject<List<SendMessageDTO>>(requestJSON);

            if (messages.Any())
            {
                var sessionID = new Random().Next(1, 9).ToString();
                var userProperties = new Dictionary<string, object>();
                userProperties.Add("FilterCondition", sessionID);
                AdditionalProperties properties = new AdditionalProperties()
                {
                    SessionID = sessionID,
                    UserProperties = userProperties

                };

                if (!string.IsNullOrEmpty(queueName))
                {
                    await ServiceBusHelper.ServiceBusHelper.SendMessageAsync<SendMessageDTO>(TypeOfContainer.Queue, queueName, messages, properties);
                }
                else if (!string.IsNullOrEmpty(topicName))
                {

                   await ServiceBusHelper.ServiceBusHelper.SendMessageAsync<SendMessageDTO>(TypeOfContainer.Topic, topicName, messages, properties);
                }
            }


            

            return new OkObjectResult("Successfully sent");
        }
    }
}
