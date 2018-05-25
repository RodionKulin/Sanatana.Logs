using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs
{
    public class LogWritersHub
    {
        //fields
        protected List<ILogWriter> _logsWriters;


        //init
        public LogWritersHub(IEnumerable<ILogWriter> logsWriters)
        {
            _logsWriters = logsWriters.ToList();
        }


        //methods
        public virtual void WritePerf(LogDetail usageInfo)
        {
            ILogWriter writer = _logsWriters.First(x => x.LogType == LogType.Perf);
            writer.Write(usageInfo);
        }
        public virtual void WriteUsage(LogDetail usageInfo)
        {
            ILogWriter writer = _logsWriters.First(x => x.LogType == LogType.Usage);
            writer.Write(usageInfo);
        }
        public virtual void WriteDiagnostic(LogDetail usageInfo)
        {
            ILogWriter writer = _logsWriters.First(x => x.LogType == LogType.Diagnostic);
            writer.Write(usageInfo);
        }
        public virtual void WriteError(LogDetail usageInfo)
        {
            ILogWriter writer = _logsWriters.First(x => x.LogType == LogType.Error);
            writer.Write(usageInfo);
        }
    }
}
