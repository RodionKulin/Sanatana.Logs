using ExampleMVC.Controllers;
using Sanatana.Logs;
using Sanatana.Logs.FileWriter;
using Sanatana.Logs.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ExampleMVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            LogsConfig.ConfigureWebLogs();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

       

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex == null)
                return;

            int httpStatus;
            string errorControllerAction;

            WebLogsHub.WebLogger.GetHttpStatus(ex, out httpStatus);
            switch (httpStatus)
            {
                case 404:
                    errorControllerAction = "NotFound";
                    break;
                default:
                    WebLogsHub.WebLogger.LogWebError(ex);
                    errorControllerAction = "Index";
                    break;
            }

            var httpContext = ((MvcApplication)sender).Context;
            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = httpStatus;
            httpContext.Response.TrySkipIisCustomErrors = true;

            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = errorControllerAction;

            var controller = new ErrorController();
            ((IController)controller).Execute(
                new RequestContext(new HttpContextWrapper(httpContext), routeData));
        }
    }
}
