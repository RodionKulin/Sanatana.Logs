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
    public class FileUsageLogWriter : ILogWriter
    {
        //fields
        protected ILogger _logger;


        //properties
        public LogType LogType { get; } = LogType.Usage;
       


        //init
        public FileUsageLogWriter()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                , "logs", "usage.txt");
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
            _logger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }
    }
}
