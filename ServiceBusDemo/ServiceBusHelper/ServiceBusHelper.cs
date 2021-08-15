using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using ServiceBusHelper.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static ServiceBusHelper.Enums.ServiceBusEnums;

namespace ServiceBusHelper
{
    public static class ServiceBusHelper
    {
        
        private static string _connectionString = Environment.GetEnvironmentVariable("serviceBusConnectionString");
        private static ManagementClient _managementClient = InstantiateManagementHelper(_connectionString);


        #region Public Methods

        #region Service Bus Management
        /// <summary>
        /// This will Create Queues
        /// </summary>
        /// <param name="quename">Provde Queue Name</param>
        /// <param name="queueDescription">provide the properties of the queues</param>
        /// <returns></returns>
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

        /// <summary>
        /// This method deletes the queues
        /// </summary>
        /// <param name="queuePath"></param>
        /// <returns></returns>
        public static async Task DeleteQueueAsync(string queuePath)
        {
            _managementClient = (_managementClient != null) ? InstantiateManagementHelper(_connectionString) : _managementClient;

            await _managementClient.DeleteQueueAsync(queuePath);
           
        }

        /// <summary>
        /// This lists all the queue details
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// This creates the topics
        /// </summary>
        /// <param name="topicPath"></param>
        /// <returns></returns>
        public static async Task CreateTopicAsync(string topicPath)
        {
            
            var description = await _managementClient.CreateTopicAsync(topicPath);
            
        }

        /// <summary>
        /// This method will create the subscription
        /// </summary>
        /// <param name="topicPath"></param>
        /// <param name="subscriptionName"></param>
        /// <returns></returns>
        public static async Task CreateSubscriptionAsync(string topicPath, string subscriptionName)
        {
            
            var description = await _managementClient.CreateSubscriptionAsync(topicPath, subscriptionName);
            
        }
        #endregion

        #region Sending Messages

        public static async Task SendMessageAsync<T>(TypeOfContainer typeOfContainer,string pathName, List<T> messages, AdditionalProperties properties=null)
        {
            try
            {
                QueueClient queueClient = null;
                TopicClient topicClient = null;
                string sessionID = (properties != null && !string.IsNullOrEmpty(properties.SessionID)) ? properties.SessionID : string.Empty;
                switch (typeOfContainer)
                {
                    case TypeOfContainer.Queue:
                        queueClient = new QueueClient(_connectionString, pathName);
                        break;
                    case TypeOfContainer.Topic:
                        topicClient = new TopicClient(_connectionString, pathName);
                        break;

                    default:
                        break;
                }

                List<Message> messageBatch = new List<Message>();

                foreach (var message in messages)
                {
                    string messageBody = JsonConvert.SerializeObject(message);
                    var encodedmsg = new Message(Encoding.UTF8.GetBytes(messageBody));

                    if (properties?.UserProperties != null)
                    {
                        foreach (var userProperty in properties.UserProperties)
                        {
                            encodedmsg.UserProperties.Add(userProperty.Key, userProperty.Value);
                        }
                    }

                    encodedmsg.SessionId = sessionID;
                    messageBatch.Add(encodedmsg);
                }

                switch (typeOfContainer)
                {
                    case TypeOfContainer.Queue:
                        await queueClient.SendAsync(messageBatch);
                        await queueClient.CloseAsync();
                        break;
                    case TypeOfContainer.Topic:
                        await topicClient.SendAsync(messageBatch);
                        await topicClient.CloseAsync();
                        break;

                    default:
                        break;
                }
            }
            catch(Exception ex)
            {

            }
            


        }
        #endregion

        #region Consuming Messages
        #endregion
        #endregion

        #region Private Methods
        /// <summary>
        /// This will create the properties of the Queues
        /// </summary>
        /// <param name="queuePath"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
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


        /// <summary>
        /// This will instantiate a management client instance
        /// </summary>
        /// <param name="_connectionString"></param>
        /// <returns></returns>
        private static ManagementClient InstantiateManagementHelper(string _connectionString)
        {
            return new ManagementClient(_connectionString);
        }
        #endregion
    }
}
