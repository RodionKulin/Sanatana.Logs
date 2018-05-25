using Sanatana.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;

namespace Sanatana.Logs.WCF.Behaviors
{
    public class TrackUsageParameterInspector : IParameterInspector
    {
        //fields
        private string _serviceName;


        //init
        public TrackUsageParameterInspector(string serviceName)
        {
            _serviceName = serviceName;
        }


        //methods
        public object BeforeCall(string operationName, object[] inputs)
        {
            return inputs ?? new object[0];
        }

        public void AfterCall(string operationName, object[] outputs, 
            object returnValue, object correlationState)
        {
            var details = new Dictionary<string, object>();

            object[] inputs = correlationState as object[];
            if (inputs != null)
            {
                for (int i = 0; i < inputs.Count(); i++)
                {
                    string value = inputs[i] == null ? "" : inputs[i].ToString();
                    details.Add($"input-{i}", value);
                }
            }

            WcfLogsHub.WcfLogger.LogUsage(_serviceName, operationName, details);
        }
    }
}
