using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace Sanatana.Logs.WCF.Behaviors
{
    public class TrackUsageBehaviorExtension : BehaviorExtensionElement
    {
        //properties
        public override Type BehaviorType => typeof(TrackUsageServiceBehaviorAttribute);

        [ConfigurationProperty("enabled")]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }



        //methods
        protected override object CreateBehavior()
        {
            return new TrackUsageServiceBehaviorAttribute(Enabled);
        }

    }
}
