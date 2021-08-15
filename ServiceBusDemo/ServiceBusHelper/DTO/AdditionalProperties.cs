using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBusHelper.DTO
{
    public class AdditionalProperties
    {
        public string SessionID { get; set; }

        public IDictionary<string,object> UserProperties { get; set; }
    }
}
