using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanatana.Logs.WF.Tracking
{
    public class CustomActivityMessage
    {
        //properties
        /// <summary>
        /// The activity name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The activity Id
        /// </summary>
        public string ActivityId { get; set; }

        /// <summary>
        /// The activity instance id
        /// </summary>
        public string ActivityInstanceId { get; set; }

        /// <summary>
        /// The activity type name
        /// </summary>
        public string TypeName { get; set; }


        //init
        public CustomActivityMessage() { }

        public CustomActivityMessage(ActivityInfo activityInfo)
        {
            if (activityInfo != null)
            {
                Name = activityInfo.Name;
                ActivityId = activityInfo.Id;
                ActivityInstanceId = activityInfo.InstanceId;
                TypeName = activityInfo.TypeName;
            }
        }

    }
}
