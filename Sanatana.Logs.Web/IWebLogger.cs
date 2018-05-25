using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;

namespace Sanatana.Logs.Web
{
    public interface IWebLogger
    {
        void GetHttpStatus(Exception ex, out int httpStatus);
        void LogWebDiagnostic(string message, Dictionary<string, object> diagnosticInfo = null);
        void LogWebError(ExceptionLoggerContext context, string correlationId = null);
        void LogWebError(Exception ex, string correlationId = null);
        void LogWebUsage(string activityName, Dictionary<string, object> additionalInfo = null);
        PerfTracker StartPerfTracker(HttpActionContext actionContext);
        PerfTracker StartPerfTracker(ActionExecutingContext filterContext);
    }
}