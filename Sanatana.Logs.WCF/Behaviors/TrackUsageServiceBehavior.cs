using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Sanatana.Logs.WCF.Behaviors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TrackUsageServiceBehaviorAttribute : Attribute, IServiceBehavior
    {
        //fields
        private bool _enabled;


        //init
        public TrackUsageServiceBehaviorAttribute(bool enabled)
        {
            _enabled = enabled;
        }


        //methods
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {
            if (_enabled)
            {
                foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
                {
                    foreach (OperationDescription op in endpoint.Contract.Operations)
                    {
                        op.OperationBehaviors.Add(new TrackUsageOperationBehaviorAttribute(_enabled));
                    }
                }                    
            }
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, 
            BindingParameterCollection bindingParameters)
        {            
        }
        
        public void Validate(ServiceDescription serviceDescription, 
            ServiceHostBase serviceHostBase)
        {            
        }
    }
}
