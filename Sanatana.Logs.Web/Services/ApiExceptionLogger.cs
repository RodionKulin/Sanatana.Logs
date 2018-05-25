using Sanatana.Logs;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http.ExceptionHandling;

namespace Sanatana.Logs.Web.Services
{
    public class ApiExceptionLogger : ExceptionLogger
    {

        //methods
        public override void Log(ExceptionLoggerContext context)
        {
            //This is here because the Logger is called BEFORE the Handler in the 
            //Web API exception pipeline  
            string correlationId = Guid.NewGuid().ToString(); 
            context.Exception.Data.Add("ErrorId", correlationId);

            WebLogsHub.WebLogger.LogWebError(context, correlationId);
        }

    }
}
