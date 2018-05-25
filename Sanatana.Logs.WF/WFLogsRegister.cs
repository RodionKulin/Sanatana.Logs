using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.WF
{
    public class WFLogsRegister
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
        /// IWFLogger factory method. Default factory will produce new instance on each call.
        /// </summary>
        public Func<IWFLogger> WFLoggerFactory { get; set; }


        //init
        public WFLogsRegister()
        {
            LogSettingsFactory = ReadConfigurationLogsSettings;
            LogWritersFactory = () => _logWriters;
            WFLoggerFactory = () => new WFLogger(LogWritersFactory(), LogSettingsFactory());
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
