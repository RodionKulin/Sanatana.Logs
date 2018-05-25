using Sanatana.Logs;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.FileWriter
{
    public class FileDiagnosticLogWriter : ILogWriter
    {
        //fields
        protected ILogger _logger;
        protected LogSettings _logsSettings;


        //properties
        public LogType LogType { get; } = LogType.Diagnostic;


        //init
        public FileDiagnosticLogWriter(LogSettings logsSettings)
        {
            _logsSettings = logsSettings;

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                , "logs", "diagnostic.txt");
            _logger = new LoggerConfiguration()
                .WriteTo.File(
                    path: path,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 20971520)
                .CreateLogger();
        }


        //methods
        public virtual void Write(LogDetail infoToLog)
        {
            if (_logsSettings.WriteDiagnostics)
            {
                return;
            }

            _logger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }
    }
}
