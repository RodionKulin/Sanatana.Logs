using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.ElasticsearchWriter
{
    public class EsLogWriterSettings
    {
        public string EsNodeUris { get; set; }
        public int BatchPostingLimit { get; set; } = 50;
    }
}
