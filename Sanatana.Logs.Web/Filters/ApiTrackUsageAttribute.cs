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
    public class ApiTrackUsageAttribute : ActionFilterAttribute
    {
        //fields
        private string _activityName;


        //init
        public ApiTrackUsageAttribute(string activityName)
        {
            _activityName = activityName;
        }


        //methods
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            WebLogsHub.WebLogger.LogWebUsage(_activityName);
        }

    }
}
