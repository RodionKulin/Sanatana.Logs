using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.MSSqlWriter
{
    public class MSSqlLogWriterSettings
    {
        public string ConnectionString { get; set; }
        public int BatchPostingLimit { get; set; } = 50;
    }
}
