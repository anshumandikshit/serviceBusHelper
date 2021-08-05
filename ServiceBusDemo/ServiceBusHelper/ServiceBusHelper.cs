using Microsoft.Azure.ServiceBus.Management;
using ServiceBusHelper.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusHelper
{
    public static class ServiceBusHelper
    {
        
        private static string _connectionString = Environment.GetEnvironmentVariable("serviceBusConnectionString");
        private static ManagementClient _managementClient = InstantiateManagementHelper(_connectionString);

        public static async Task<string> CreateQueueAsync(string quename, QueueDescriptorDTO queueDescription)
        {
            _managementClient = (_managementClient!=null) ? InstantiateManagementHelper(_connectionString) : _managementClient;

            
            var description = GetQueueDescription(quename, queueDescription);
            var createdDescription = await _managementClient.CreateQueueAsync(description);
            if (createdDescription != null)
            {
                return $"Queue {quename} created Successfully";
            }

            return "something went wrong";
        }

        public static async Task DeleteQueueAsync(string queuePath)
        {
            _managementClient = (_managementClient != null) ? InstantiateManagementHelper(_connectionString) : _managementClient;

            await _managementClient.DeleteQueueAsync(queuePath);
           
        }

        public static async Task<List<string>> ListQueuesAsync()
        {
            _managementClient = (_managementClient != null) ? InstantiateManagementHelper(_connectionString) : _managementClient;
            IEnumerable<QueueDescription> queueDescriptions = await _managementClient.GetQueuesAsync();

            List<string> results = new List<string>();

            foreach (QueueDescription queueDescription in queueDescriptions)
            {
                results.Add(queueDescription.Path);
               
            }

            return results;
            
        }

        public static async Task CreateTopicAsync(string topicPath)
        {
            
            var description = await _managementClient.CreateTopicAsync(topicPath);
            
        }

        public static async Task CreateSubscriptionAsync(string topicPath, string subscriptionName)
        {
            
            var description = await _managementClient.CreateSubscriptionAsync(topicPath, subscriptionName);
            
        }

        private static QueueDescription GetQueueDescription(string queuePath, QueueDescriptorDTO dto)
        {
            return new QueueDescription(queuePath)
            {
                RequiresDuplicateDetection = dto.RequiresDuplicateDetection,
                //DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10),
                RequiresSession = dto.RequiresSession,
                MaxDeliveryCount = dto.MaxDeliveryCount,
                //DefaultMessageTimeToLive = TimeSpan.FromHours(1),
                EnableDeadLetteringOnMessageExpiration = dto.EnableDeadLetteringOnMessageExpiration
            };
        }

        private static ManagementClient InstantiateManagementHelper(string _connectionString)
        {
            return new ManagementClient(_connectionString);
        }
    }
}
