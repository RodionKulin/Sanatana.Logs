using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace Sanatana.Logs.Web.Services
{
    public class ApiExceptionHandler : ExceptionHandler
    {
        //methods
        public override bool ShouldHandle(ExceptionHandlerContext context)
        {
            return true;
        }
        
        public override void Handle(ExceptionHandlerContext context)
        {
            string errorInfo = string.Empty;

            // this is set within the custom *logger* which is called BEFORE this 
            // in the exception pipeline
            if (context.Exception.Data.Contains("ErrorId")) 
                errorInfo = " Error ID: " + context.Exception.Data["ErrorId"];
            
            context.Result = new TextPlainErrorResult
            {               
                Request = context.ExceptionContext.Request,
                Content = "There was an error during processing a request." + errorInfo                
            };
        }
        
    }
}
