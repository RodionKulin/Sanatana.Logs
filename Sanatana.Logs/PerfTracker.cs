using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Sanatana.Logs
{
    public class PerfTracker
    {
        //fields
        private Stopwatch _sw;
        private LogDetail _perfLog;
        private LogWritersHub _logsWriter;


        //init
        public PerfTracker(LogWritersHub logsWriter, LogDetail perfLog)
        {
            _logsWriter = logsWriter;
            _perfLog = perfLog;

            _sw = Stopwatch.StartNew();

            DateTime beginTime = DateTime.UtcNow;
            _perfLog.AdditionalInfo = new Dictionary<string, object>()
            {
                { "StartedUtc", beginTime.ToString(CultureInfo.InvariantCulture) + " UTC" }
            };
        }


        //methods
        public void Stop()
        {
            _sw.Stop();
            _perfLog.ElapsedMilliseconds = _sw.ElapsedMilliseconds;

            _logsWriter.WritePerf(_perfLog);
        }
    }
}
