using System;
using System.Collections.Generic;
using System.Text;

namespace Sanatana.Logs
{
    public class LogDetail
    {
        public DateTime TimestampUtc { get; set; }
        public string Message { get; set; }

        // WHERE
        public string Product { get; set; }
        public string Layer { get; set; }
        public string Location { get; set; }
        public string Hostname { get; set; }

        // WHO
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        // EVERYTHING ELSE
        public string CorrelationId { get; set; } // exception shielding from server to client
        public long? ElapsedMilliseconds { get; set; }  // only for performance entries
        public Dictionary<string, object> AdditionalInfo { get; set; }  // catch-all for anything else
        public Exception Exception { get; set; }  // the exception for error logging


        //init
        public LogDetail()
        {
            TimestampUtc = DateTime.UtcNow;
            AdditionalInfo = new Dictionary<string, object>();
        }
    }
}
