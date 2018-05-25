using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.ElasticsearchWriter
{
    public class EsUsageLogWriter : ILogWriter
    {
        //fields
        protected ILogger _logger;
        protected EsLogWriterSettings _esLogsWriterSettings;


        //properties
        public LogType LogType { get; } = LogType.Usage;
       


        //init
        public EsUsageLogWriter(EsLogWriterSettings settings)
        {
            _esLogsWriterSettings = settings;

            _logger = new LoggerConfiguration()
                .WriteTo.Elasticsearch(settings.EsNodeUris,
                    indexFormat: "usage-{0:yyyy.MM.dd}",
                    inlineFields: true,
                    batchPostingLimit: settings.BatchPostingLimit)
                .CreateLogger();
        }


        //methods
        public virtual void Write(LogDetail infoToLog)
        {
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
