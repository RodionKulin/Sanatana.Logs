using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.Web
{
    public static class WebLogsHub
    {
        //fields
        private static WebLogsRegister _register;


        //properties
        public static LogSettings LogsSettings
        {
            get
            {
                return _register.LogSettingsFactory();
            }
        }
        public static LogWritersHub LogWritersHub
        {
            get
            {
                IEnumerable<ILogWriter> logWriters = _register.LogWritersFactory();
                return new LogWritersHub(logWriters);
            }
        }
        public static IWebLogger WebLogger
        {
            get
            {
                return _register.WebLoggerFactory();
            }
        }


        //methods
        public static void Configure(WebLogsRegister register)
        {
            _register = register;
        }

    }
}
