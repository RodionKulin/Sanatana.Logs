using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.WF
{
    public static class WFLogsHub
    {
        //fields
        private static WFLogsRegister _register;


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
        public static IWFLogger WFLogger
        {
            get
            {
                return _register.WebLoggerFactory();
            }
        }


        //methods
        public static void Configure(WFLogsRegister register)
        {
            _register = register;
        }

    }
}
