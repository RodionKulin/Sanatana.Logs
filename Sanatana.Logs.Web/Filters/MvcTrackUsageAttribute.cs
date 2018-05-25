using System.Web.Mvc;

namespace Sanatana.Logs.Web.Filters
{
    public class MvcTrackUsageAttribute : ActionFilterAttribute
    {
        //fields
        private string _activityName;


        //init
        public MvcTrackUsageAttribute(string activityName)
        {
            _activityName = activityName;
        }


        //methods
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            WebLogsHub.WebLogger.LogWebUsage(_activityName);
        }
    }
}
