using Sanatana.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;

namespace Sanatana.Logs.WCF.Behaviors
{
    public class TrackPerfParameterInspector : IParameterInspector
    {
        //fields
        private string _serviceName;


        //init
        public TrackPerfParameterInspector(string serviceName)
        {
            _serviceName = serviceName;
        }


        //methods
        public object BeforeCall(string operationName, object[] inputs)
        {
            var details = new Dictionary<string, object>();
            if(inputs != null)
            {
                for (int i = 0; i < inputs.Count(); i++)
                {
                    string value = inputs[i] == null ? "" : inputs[i].ToString();
                    details.Add($"input-{i}", value);
                }
            }

            PerfTracker perfTracker = WcfLogsHub.WcfLogger
                .StartPerfTracker(_serviceName, operationName, details);
            return perfTracker;
        }

        public void AfterCall(string operationName, object[] outputs, 
            object returnValue, object correlationState)
        {
            PerfTracker perfTracker = correlationState as PerfTracker;
            if (perfTracker == null)
                return;

            perfTracker.Stop();
        }
    }
}
