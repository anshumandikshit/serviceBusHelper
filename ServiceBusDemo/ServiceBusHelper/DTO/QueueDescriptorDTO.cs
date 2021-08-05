using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBusHelper.DTO
{
    public class QueueDescriptorDTO
    {
        public bool RequiresDuplicateDetection { get; set; }
        public bool RequiresSession { get; set; }
        public int MaxDeliveryCount { get; set; }
        public bool EnableDeadLetteringOnMessageExpiration { get; set; }
        
    }
}
