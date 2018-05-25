
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.MSSqlWriter
{
    public class MSSqlWriterDiagnosticLogWriter : MSSqlBaseLogWriter, ILogWriter
    {
        //fields
        protected ILogger _logger;
        protected LogSettings _logsSettings;
        protected MSSqlLogWriterSettings _msSqlLogsWriterSettings;


        //properties
        public LogType LogType { get; } = LogType.Diagnostic;


        //init
        public MSSqlWriterDiagnosticLogWriter(LogSettings logsSettings, MSSqlLogWriterSettings settings)
        {
            _logsSettings = logsSettings;
            _msSqlLogsWriterSettings = settings;

            _logger = new LoggerConfiguration()
                .WriteTo.MSSqlServer(settings.ConnectionString, "DiagnosticLogs",
                    autoCreateSqlTable: true,
                    columnOptions: GetSqlColumnOptions(),
                    batchPostingLimit: settings.BatchPostingLimit)
                .CreateLogger();
        }


        //methods
        public virtual void Write(LogDetail infoToLog)
        {
            if (_logsSettings.WriteDiagnostics)
            {
                return;
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
    }
}
