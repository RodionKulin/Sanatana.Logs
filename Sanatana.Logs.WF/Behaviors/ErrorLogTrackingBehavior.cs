using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Tracking.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Sanatana.Logs.WF.Behaviors
{
    public class ErrorLogTrackingBehavior : IServiceBehavior
    {
        //fields
        private string _profileName;
        private bool _enabled;
        private ILogger _logger;


        //init
        public ErrorLogTrackingBehavior(ILogger logger, string profileName, bool enabled)
        {
            _logger = logger;
            _profileName = profileName;
            _enabled = enabled;
        }


        //methods
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            WorkflowServiceHost workflowServiceHost = serviceHostBase as WorkflowServiceHost;
            if (workflowServiceHost != null && _enabled)
            {
                string workflowDisplayName = workflowServiceHost.Activity.DisplayName;
                TrackingProfile trackingProfile = GetProfile(_profileName, workflowDisplayName);

                workflowServiceHost.WorkflowExtensions.Add(()
                    => new ErrorLogTrackingParticipant(_logger)
                    {
                        TrackingProfile = trackingProfile
                    });
            }
        }

        private TrackingProfile GetProfile(string trackingProfileName, string displayName)
        {
            TrackingProfile trackingProfile;

            TrackingSection trackingSection = (TrackingSection)WebConfigurationManager.GetSection("system.serviceModel/tracking");
            if (trackingSection == null)
            {
                return null;
            }

            if (trackingProfileName == null)
            {
                trackingProfileName = "";
            }

            //Find the profile with the specified profile name in the list of profile found in config
            IEnumerable<TrackingProfile> match;
            match = from p in new List<TrackingProfile>(trackingSection.TrackingProfiles)
                    where (p.Name == trackingProfileName) 
                        && ((p.ActivityDefinitionId == displayName) || (p.ActivityDefinitionId == "*"))
                    select p;

            if (match.Count() == 0)
            {
                //return an empty profile
                trackingProfile = new TrackingProfile
                {
                    ActivityDefinitionId = displayName
                };
            }
            else
            {
                trackingProfile = match.First();
            }

            return trackingProfile;
        }


        //not used methods
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}
