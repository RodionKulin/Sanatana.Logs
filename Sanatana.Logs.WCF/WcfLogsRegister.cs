using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.WCF
{
    public class WcfLogsRegister
    {
        //fields
        protected List<ILogWriter> _logWriters = new List<ILogWriter>();


        //properties
        /// <summary>
        /// LogSettings factory method. Default factory will read value from application configuration on each call.
        /// </summary>
        public Func<LogSettings> LogSettingsFactory { get; set; }

        /// <summary>
        /// ILogWriter list factory method. Default factory will reuse single instance of ILogWriter list.
        /// </summary>
        public Func<IEnumerable<ILogWriter>> LogWritersFactory { get; set; }

        /// <summary>
        /// IWCFLogger factory method. Default factory will produce new instance on each call.
        /// </summary>
        public Func<IWcfLogger> WcfLoggerFactory { get; set; }


        //init
        public WcfLogsRegister()
        {
            LogSettingsFactory = ReadConfigurationLogsSettings;
            LogWritersFactory = () => _logWriters;
            WcfLoggerFactory = () => new WcfLogger(LogWritersFactory(), LogSettingsFactory());
        }



        //methods
        protected virtual LogSettings ReadConfigurationLogsSettings()
        {
            string product = ConfigurationManager.AppSettings["Logs_Product"];
            string layer = ConfigurationManager.AppSettings["Logs_Layer"];
            string writeDiagnostics = ConfigurationManager.AppSettings["Logs_WriteDiagnostics"];

            return new LogSettings
            {
                Product = product,
                Layer = layer,
                WriteDiagnostics = Convert.ToBoolean(writeDiagnostics)
            };
        }

        /// <summary>
        /// Register LogWriter to resolve from default LogWritersFactory.
        /// </summary>
        /// <param name="logsWriter"></param>
        public virtual void RegisterLogWriter(ILogWriter logsWriter)
        {
            _logWriters.RemoveAll(x => x.LogType == logsWriter.LogType);
            _logWriters.Add(logsWriter);
        }

    }
}
