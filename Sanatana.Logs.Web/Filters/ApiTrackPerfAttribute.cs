using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Sanatana.Logs;

namespace Sanatana.Logs.Web.Filters
{
    public class ApiTrackPerfAttribute : ActionFilterAttribute
    {
        //methods
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.Request.Properties["PerfTracker"] = WebLogsHub.WebLogger.StartPerfTracker(actionContext);
        }        

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            try
            {
                PerfTracker perfTracker = actionExecutedContext.Request.Properties["PerfTracker"] 
                    as PerfTracker;

                if (perfTracker != null)
                    perfTracker.Stop();
            }
            catch (Exception)
            {
                // ignoring logging exceptions -- don't want an API call to fail 
                // if we run into logging problems. 
            }
        }                
    }
}
