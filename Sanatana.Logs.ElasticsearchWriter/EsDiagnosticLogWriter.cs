using Sanatana.Logs;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.ElasticsearchWriter
{
    public class EsDiagnosticLogWriter : ILogWriter
    {
        //fields
        protected ILogger _logger;
        protected LogSettings _logsSettings;
        protected EsLogWriterSettings _esLogsWriterSettings;


        //properties
        public LogType LogType { get; } = LogType.Diagnostic;


        //init
        public EsDiagnosticLogWriter(LogSettings logsSettings, EsLogWriterSettings settings)
        {
            _logsSettings = logsSettings;
            _esLogsWriterSettings = settings;

            _logger = new LoggerConfiguration()
                .WriteTo.Elasticsearch(settings.EsNodeUris,
                    indexFormat: "diagnostic-{0:yyyy.MM.dd}",
                    inlineFields: true,
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
                    infoToLog.Layer, infoToLog.Location,
                    infoToLog.Product,
                    infoToLog.ElapsedMilliseconds, infoToLog.Exception?.ToExceptionString(),
                    infoToLog.Hostname, infoToLog.UserId,
                    infoToLog.UserName, infoToLog.CorrelationId,
                    infoToLog.AdditionalInfo
                    );
        }
    }
}
