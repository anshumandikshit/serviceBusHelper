using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBusHelper.DTO
{
    public class SendMessageDTO
    {
        [JsonProperty("messageID")]
        public int MessageID { get; set; }

        [JsonProperty("messageDescription")]
        public string MessageDescription { get; set; }
    }
}
