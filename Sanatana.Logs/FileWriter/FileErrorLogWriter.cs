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
    public class FileErrorLogWriter : ILogWriter
    {
        //fields
        protected ILogger _logger;


        //properties
        public LogType LogType { get; } = LogType.Error;


        //init
        public FileErrorLogWriter()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory
                , "logs", "error.txt");
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
            if (infoToLog.Exception != null)
            {
                string procName = FindProcName(infoToLog.Exception);
                infoToLog.Location = string.IsNullOrEmpty(procName) ? infoToLog.Location : procName;
                infoToLog.Message = GetMessageFromException(infoToLog.Exception);
            }

            _logger.Write(LogEventLevel.Information, "{@LogDetail}", infoToLog);
        }
        protected virtual string FindProcName(Exception ex)
        {
            var sqlEx = ex as System.Data.SqlClient.SqlException;
            if (sqlEx != null)
            {
                string procName = sqlEx.Procedure;
                if (!string.IsNullOrEmpty(procName))
                    return procName;
            }

            if (!string.IsNullOrEmpty((string)ex.Data["Procedure"]))
            {
                return (string)ex.Data["Procedure"];
            }

            if (ex.InnerException != null)
                return FindProcName(ex.InnerException);

            return null;
        }
        protected virtual string GetMessageFromException(Exception ex)
        {
            if (ex.InnerException != null)
                return GetMessageFromException(ex.InnerException);

            return ex.Message;
        }
    }
}
