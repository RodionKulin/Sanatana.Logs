using System.Web;
using System.Web.Mvc;
using Sanatana.Logs.Web.Filters;

namespace ExampleMVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MvcTrackPerfAttribute());

        }
    }
}
