using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.WF.Tracking
{
    public class ErrorLogTrackingParticipant : TrackingParticipant
    {
        //methods
        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            if(record is WorkflowInstanceUnhandledExceptionRecord)
            {
                var exceptionRecord = (WorkflowInstanceUnhandledExceptionRecord)record;
                WFLogsHub.WFLogger.
                _logger.LogError(exceptionRecord.UnhandledException);
            }
        }
    }
}
