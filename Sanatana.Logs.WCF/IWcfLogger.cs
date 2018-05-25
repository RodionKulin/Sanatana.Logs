using System;

namespace Sanatana.Logs.WCF
{
    public interface IWcfLogger
    {
        void LogDiagnostic(string message, object additionalInfo = null);
        void LogError(string message, Exception ex, object additionalInfo = null);
        void LogUsage(string serviceName, string operationName, object additionalInfo = null);
        PerfTracker StartPerfTracker(string serviceName, string operationName, object additionalInfo = null);
    }
}