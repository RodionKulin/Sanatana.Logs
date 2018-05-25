using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.WCF
{
    public static class WcfLogsHub
    {
        //fields
        private static WcfLogsRegister _register;


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
        public static IWcfLogger WcfLogger
        {
            get
            {
                return _register.WcfLoggerFactory();
            }
        }


        //methods
        public static void Configure(WcfLogsRegister register)
        {
            _register = register;
        }
    }
}
