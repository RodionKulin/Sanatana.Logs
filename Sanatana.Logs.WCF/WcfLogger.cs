using Sanatana.Logs.WCF.Behaviors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Linq;

namespace Sanatana.Logs.WCF
{
    public class WcfLogger : IWcfLogger
    {
        //fields
        protected LogWritersHub _logsWriter;
        protected LogSettings _logsSettings;


        //init
        public WcfLogger(IEnumerable<ILogWriter> writers, LogSettings logsSettings)
        {
            _logsWriter = new LogWritersHub(writers);
            _logsSettings = logsSettings;
        }


        //methods
        public virtual void LogDiagnostic(string message, object additionalInfo = null)
        {
            LogDetail logEntry = GetWcfLogEntry(message, additionalInfo);
            _logsWriter.WriteUsage(logEntry);
        }

        public virtual void LogError(string message, Exception ex
            , object additionalInfo = null)
        {
            LogDetail logEntry = GetWcfLogEntry(message, additionalInfo, ex: ex);

            if (ex.Data.Contains("ErrorId"))
                logEntry.CorrelationId = ex.Data["ErrorId"].ToString();

            _logsWriter.WriteError(logEntry);
        }
        
        public virtual void LogUsage(string serviceName, string operationName
            , object additionalInfo = null)
        {
            LogDetail logEntry = GetWcfLogEntry(operationName, additionalInfo, serviceName: serviceName);
            _logsWriter.WriteUsage(logEntry);
        }

        public virtual PerfTracker StartPerfTracker(string serviceName, string operationName
            , object additionalInfo = null)
        {
            LogDetail logDetail = GetWcfLogEntry(operationName, additionalInfo, serviceName: serviceName);
            return new PerfTracker(_logsWriter, logDetail);
        }

        protected virtual LogDetail GetWcfLogEntry(string message, object additionalInfo,
            string serviceName = null, Exception ex = null)
        {
            var logEntry = new LogDetail
            {
                UserId = "",
                UserName = "",
                TimestampUtc = DateTime.UtcNow,
                Product = _logsSettings.Product,
                Layer = _logsSettings.Layer,
                Location = serviceName,
                Message = message,
                Hostname = Environment.MachineName,
                Exception = ex
            };

            if (string.IsNullOrEmpty(logEntry.Location))
                logEntry.Location = GetLocationFromExceptionOrStackTrace(logEntry);

            logEntry.AdditionalInfo = new Dictionary<string, object>();
            if (additionalInfo != null)
                SetPropertiesFromAdditionalInfo(logEntry, additionalInfo);

            return logEntry;
        }

        protected virtual string GetLocationFromExceptionOrStackTrace(LogDetail logEntry)
        {
            StackTrace st = logEntry.Exception != null
                ? new StackTrace(logEntry.Exception)
                : new StackTrace();

            StackFrame[] sf = st.GetFrames();
            foreach (StackFrame frame in sf)
            {
                MethodBase method = frame.GetMethod();
                if (method.DeclaringType == typeof(WcfLogger) ||
                    method.DeclaringType == typeof(TrackUsageParameterInspector))
                {
                    continue;
                }

                return $"{method.DeclaringType.FullName}->{method.Name}";
            }

            return "Unable to determine location from StackTrace";
        }

        protected virtual void SetPropertiesFromAdditionalInfo(LogDetail logEntry, object additionalInfo)
        {
            if (additionalInfo is Dictionary<string, object>)
            {
                var ai = additionalInfo as Dictionary<string, object>;

                foreach (KeyValuePair<string, object> item in ai)
                {
                    if (!logEntry.AdditionalInfo.ContainsKey(item.Key))
                        logEntry.AdditionalInfo[item.Key] = item.Value;
                }
            }
            else  // not a dictionary
            {
                PropertyInfo[] props = additionalInfo.GetType().GetProperties();
                foreach (PropertyInfo prop in props)
                {
                    object propertyValue = prop.GetValue(additionalInfo);
                    logEntry.AdditionalInfo[$"dtl-{prop.Name}"] = propertyValue.ToString();
                }
            }
        }        
    }
}
