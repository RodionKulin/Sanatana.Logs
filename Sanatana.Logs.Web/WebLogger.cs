using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;

namespace Sanatana.Logs.Web
{
    public class WebLogger : IWebLogger
    {
        //fields
        protected LogWritersHub _logsWriter;
        protected LogSettings _logsSettings;


        //init
        public WebLogger(IEnumerable<ILogWriter> writers, LogSettings logsSettings)
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

        public virtual PerfTracker StartPerfTracker(ActionExecutingContext filterContext)
        {
            string userId, userName, location;
            Dictionary<string, object> dict = GetWebLoggingData(out userId, out userName, out location);

            string type = filterContext.HttpContext.Request.RequestType;
            string perfName = filterContext.ActionDescriptor.ActionName + "_" + type;

            var perfLog = new LogDetail()
            {
                Message = perfName,
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

        public virtual void LogWebError(Exception ex, string correlationId = null)
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

        public virtual void LogWebError(ExceptionLoggerContext context, string correlationId = null)
        {
            var dict = new Dictionary<string, object>();

            string userId, userName;
            ClaimsPrincipal user = context.RequestContext.Principal as ClaimsPrincipal;
            GetUserData(dict, user, out userId, out userName);

            string location;
            GetLocationForApiCall(context.RequestContext, dict, out location);

            var logEntry = new LogDetail()
            {
                Product = _logsSettings.Product,
                Layer = _logsSettings.Layer,
                Location = location,
                Hostname = Environment.MachineName,
                TimestampUtc = DateTime.UtcNow,
                CorrelationId = correlationId ?? HttpContext.Current.Session?.SessionID,
                Exception = context.Exception,
                UserId = userId,
                UserName = userName,
                AdditionalInfo = dict
            };

            _logsWriter.WriteError(logEntry);
        }

        public virtual void GetHttpStatus(Exception ex, out int httpStatus)
        {
            httpStatus = 500;  // default is server error
            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;
                httpStatus = httpEx.GetHttpCode();
            }
        }
        
        protected virtual Dictionary<string, object> GetWebLoggingData(out string userId, 
            out string userName, out string location)
        {
            var data = new Dictionary<string, object>();
            
            GetRequestData(data, out location);
            GetUserData(data, ClaimsPrincipal.Current, out userId, out userName);
            GetSessionData(data);
            // got cookies?  

            return data;
        }

        protected virtual void GetRequestData(Dictionary<string, object> data, out string location)
        {
            location = "";
            HttpRequest request = HttpContext.Current.Request;
            
            if (request != null)
            {
                location = request.Path;

                // MS Edge requires special detection logic
                string type, version;
                GetBrowserInfo(request, out type, out version);

                data.Add("Browser", $"{type}{version}");
                data.Add("UserAgent", request.UserAgent);
                data.Add("Languages", request.UserLanguages);  // non en-US preferences here??
                foreach (string qsKey in request.QueryString.Keys)
                {
                    data.Add(string.Format("QueryString-{0}", qsKey),
                        request.QueryString[qsKey.ToString()]);
                }
            }
        }

        protected virtual void GetBrowserInfo(HttpRequest request, out string type, out string version)
        {
            type = request.Browser.Type;
            if (type.StartsWith("Chrome") && request.UserAgent.Contains("Edge/"))
            {
                type = "Edge";
                version = " (v " + request.UserAgent
                    .Substring(request.UserAgent.IndexOf("Edge/") + 5) + ")";
            }
            else
            {
                version = " (v " + request.Browser.MajorVersion + "." +
                    request.Browser.MinorVersion + ")";
            }
        }

        protected virtual void GetUserData(Dictionary<string, object> data, 
            ClaimsPrincipal user, out string userId, out string userName)
        {
            userId = "";
            userName = "";

            if (user != null)
            {
                var i = 1; // i included in dictionary key to ensure uniqueness
                foreach (Claim claim in user.Claims)
                {
                    if (claim.Type == ClaimTypes.NameIdentifier)
                        userId = claim.Value;
                    else if (claim.Type == ClaimTypes.Name)
                        userName = claim.Value;
                    else
                        // example dictionary key: UserClaim-4-role 
                        data.Add(string.Format("UserClaim-{0}-{1}", i++, claim.Type), 
                            claim.Value);
                }
            }
        }

        protected virtual void GetLocationForApiCall(HttpRequestContext requestContext,
            Dictionary<string, object> dict, out string location)
        {
            // example route template: api/{controller}/{id}
            string routeTemplate = requestContext.RouteData.Route.RouteTemplate;
            HttpMethod method = requestContext.Url.Request.Method;  // GET, POST, etc.

            foreach (string key in requestContext.RouteData.Values.Keys)
            {
                string value = requestContext.RouteData.Values[key].ToString();
                long numeric;
                if (Int64.TryParse(value, out numeric)) 
                    dict.Add($"Route-{key}", value.ToString());
                else                
                    routeTemplate = routeTemplate.Replace("{" + key + "}", value);                
            }

            location = $"{method} {routeTemplate}";

            NameValueCollection qs = HttpUtility.ParseQueryString(requestContext.Url.Request.RequestUri.Query);
            int i = 0;
            foreach (string key in qs.Keys)
            {
                var newKey = string.Format("q-{0}-{1}", i++, key);
                if (!dict.ContainsKey(newKey))
                    dict.Add(newKey, qs[key]);
            }

            Uri referrer = requestContext.Url.Request.Headers.Referrer;
            if (referrer != null)
            {
                string source = referrer.OriginalString;
                if (source.ToLower().Contains("swagger"))
                    source = "Swagger";
                if (!dict.ContainsKey("Referrer"))
                    dict.Add("Referrer", source);
            }
        }

        protected virtual void GetSessionData(Dictionary<string, object> data)
        {
            if (HttpContext.Current.Session != null)
            {
                foreach (string key in HttpContext.Current.Session.Keys)
                {
                    if (HttpContext.Current.Session[key] != null)
                    {
                        data.Add(string.Format("Session-{0}", key), 
                            HttpContext.Current.Session[key].ToString());
                    }
                }
                data.Add("SessionId", HttpContext.Current.Session.SessionID);
            }
        }                        
    }
}
