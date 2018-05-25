using System.Web.Mvc;
using System.Collections.Generic;
using System;

namespace Sanatana.Logs.Web.Filters
{
    public class MvcTrackPerfAttribute : ActionFilterAttribute
    {
        //methods
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Items["Stopwatch"] = WebLogsHub.WebLogger.StartPerfTracker(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            PerfTracker stopwatch = (PerfTracker)filterContext.HttpContext.Items["Stopwatch"];

            if (stopwatch != null)
            {
                stopwatch.Stop();
            }
        }
    }
}
