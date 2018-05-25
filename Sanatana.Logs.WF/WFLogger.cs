using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;

namespace Sanatana.Logs.WF
{
    public class WFLogger : IWFLogger
    {
        //fields
        protected LogWritersHub _logsWriter;
        protected LogSettings _logsSettings;


        //init
        public WFLogger(IEnumerable<ILogWriter> writers, LogSettings logsSettings)
        {
            _logsWriter = new LogWritersHub(writers);
            _logsSettings = logsSettings;
        }


        //methods
        public virtual void LogWebUsage(string activityName, 
            Dictionary<string, object> additionalInfo = null)
        {
            string userId, userName, location;
            Dictionary<string, object> webInfo = GetWebLoggingData(out userId, out userName, out location);

            if (additionalInfo != null)
            {
                foreach (string key in additionalInfo.Keys)
                    webInfo.Add($"Info-{key}", additionalInfo[key]);
            }

            var usageInfo = new LogDetail()
            {
                Product = _logsSettings.Product,
                Layer = _logsSettings.Layer,
                TimestampUtc = DateTime.UtcNow,
                Location = location,
                UserId = userId,
                UserName = userName,
                Hostname = Environment.MachineName,
                CorrelationId = HttpContext.Current.Session?.SessionID,
                Message = activityName,
                AdditionalInfo = webInfo
            };

            _logsWriter.WriteUsage(usageInfo);
        }

        public virtual PerfTracker StartPerfTracker(HttpActionContext actionContext)
        {
            var dict = new Dictionary<string, object>();

            string userId, userName;
            ClaimsPrincipal user = actionContext.RequestContext.Principal as ClaimsPrincipal;
            GetUserData(dict, user, out userId, out userName);

            string location;
            GetLocationForApiCall(actionContext.RequestContext, dict, out location);

            var perfLog = new LogDetail()
            {
                Message = location,
                UserId = userId,
                UserName = userName,
                Product = _logsSettings.Product,
                Layer = _logsSettings.Layer,
                Location = location,
                Hostname = Environment.MachineName
            };
            foreach (KeyValuePair<string, object> item in dict)
            {
                perfLog.AdditionalInfo.Add("input-" + item.Key, item.Value);
            }

            return new PerfTracker(_logsWriter, perfLog);
        }

        public virtual void LogWebDiagnostic(string message, 
            Dictionary<string, object> diagnosticInfo = null)
        { 
            // doing this to avoid going through all the data - user, session, etc.
            if (_logsSettings.WriteDiagnostics)
            {
                return;
            }

            string userId, userName, location;
            Dictionary<string, object> webInfo = GetWebLoggingData(out userId, out userName, out location);
            if (diagnosticInfo != null)
            {
                foreach (var key in diagnosticInfo.Keys)
                    webInfo.Add(key, diagnosticInfo[key]);
            }

            var diagInfo = new LogDetail()
            {
                Product = _logsSettings.Product,
                Layer = _logsSettings.Layer,
                Location = location,
                TimestampUtc = DateTime.UtcNow,
                UserId = userId,
                UserName = userName,
                Hostname = Environment.MachineName,
                CorrelationId = HttpContext.Current.Session?.SessionID,
                Message = message,
                AdditionalInfo = webInfo
            };

            _logsWriter.WriteDiagnostic(diagInfo);
        }

        public virtual void LogWFError(TrackingRecord record)
        {
            string userId, userName, location;
            Dictionary<string, object> webInfo = GetWebLoggingData(out userId, out userName, out location);

            var errorInformation = new LogDetail()
            {
                Product = _logsSettings.Product,
                Layer = _logsSettings.Layer,
                Location = location,
                TimestampUtc = DateTime.UtcNow,
                UserId = userId,
                UserName = userName,
                Hostname = Environment.MachineName,
                CorrelationId = correlationId ?? HttpContext.Current.Session?.SessionID,
                Exception = ex,
                AdditionalInfo = webInfo
            };

            _logsWriter.WriteError(errorInformation);
        }



    }
}
