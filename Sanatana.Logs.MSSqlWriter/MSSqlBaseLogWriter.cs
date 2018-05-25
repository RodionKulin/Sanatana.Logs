using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace Sanatana.Logs.MSSqlWriter
{
    public abstract class MSSqlBaseLogWriter
    {
        //methods
        protected virtual ColumnOptions GetSqlColumnOptions()
        {
            var colOptions = new ColumnOptions();
            colOptions.Store.Remove(StandardColumn.Properties);
            colOptions.Store.Remove(StandardColumn.MessageTemplate);
            colOptions.Store.Remove(StandardColumn.Message);
            colOptions.Store.Remove(StandardColumn.Exception);
            colOptions.Store.Remove(StandardColumn.TimeStamp);
            colOptions.Store.Remove(StandardColumn.Level);

            colOptions.AdditionalDataColumns = new Collection<DataColumn>
            {
                new DataColumn {DataType = typeof(DateTime), ColumnName = "TimestampUtc"},
                new DataColumn {DataType = typeof(string), ColumnName = "Product"},
                new DataColumn {DataType = typeof(string), ColumnName = "Layer"},
                new DataColumn {DataType = typeof(string), ColumnName = "Location"},
                new DataColumn {DataType = typeof(string), ColumnName = "Message"},
                new DataColumn {DataType = typeof(string), ColumnName = "Hostname"},
                new DataColumn {DataType = typeof(string), ColumnName = "UserId"},
                new DataColumn {DataType = typeof(string), ColumnName = "UserName"},
                new DataColumn {DataType = typeof(string), ColumnName = "Exception"},
                new DataColumn {DataType = typeof(int), ColumnName = "ElapsedMilliseconds"},
                new DataColumn {DataType = typeof(string), ColumnName = "CorrelationId"},
                new DataColumn {DataType = typeof(string), ColumnName = "AdditionalInfo"},
            };

            return colOptions;
        }
    }
}
