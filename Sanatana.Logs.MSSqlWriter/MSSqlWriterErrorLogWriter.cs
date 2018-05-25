using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.MSSqlWriter
{
    public class MSSqlWriterErrorLogWriter : MSSqlBaseLogWriter, ILogWriter
    {
        //fields
        protected ILogger _logger;
        protected MSSqlLogWriterSettings _msSqlLogsWriterSettings;


        //properties
        public LogType LogType { get; } = LogType.Error;


        //init
        public MSSqlWriterErrorLogWriter(MSSqlLogWriterSettings settings)
        {
            _msSqlLogsWriterSettings = settings;

            _logger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(settings.ConnectionString, "ErrorLogs", 
                    autoCreateSqlTable: true,
                    columnOptions: GetSqlColumnOptions(), 
                    batchPostingLimit: settings.BatchPostingLimit)
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

            _logger.Write(LogEventLevel.Information,
                    "{TimestampUtc}{Message}{Layer}{Location}{Product}" +
                    "{ElapsedMilliseconds}{Exception}{Hostname}" +
                    "{UserId}{UserName}{CorrelationId}{AdditionalInfo}",
                    infoToLog.TimestampUtc, infoToLog.Message,
                    infoToLog.Layer, infoToLog.Location, infoToLog.Product,
                    infoToLog.ElapsedMilliseconds, infoToLog.Exception?.ToExceptionString(),
                    infoToLog.Hostname, infoToLog.UserId,
                    infoToLog.UserName, infoToLog.CorrelationId,
                    infoToLog.AdditionalInfo
                    );
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
