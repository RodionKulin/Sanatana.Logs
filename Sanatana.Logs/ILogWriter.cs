using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs
{
    public interface ILogWriter
    {
        LogType LogType { get; }
        void Write(LogDetail infoToLog);
    }
}
