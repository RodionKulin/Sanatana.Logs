using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Sanatana.Logs.WCF.Behaviors
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TrackUsageOperationBehaviorAttribute : Attribute, IOperationBehavior
    {
        //fields
        private bool _enabled;


        //init
        public TrackUsageOperationBehaviorAttribute(bool enabled)
        {
            _enabled = enabled;
        }


        //methods
        public void ApplyDispatchBehavior(OperationDescription operationDescription, 
            DispatchOperation dispatchOperation)
        {
            Type p = dispatchOperation.Parent.Type;

            if (_enabled && p != null)
            {
                dispatchOperation.ParameterInspectors.Add(
                    new TrackUsageParameterInspector(dispatchOperation.Parent.Type.FullName));
            }
        }

        public void AddBindingParameters(OperationDescription operationDescription, 
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, 
            ClientOperation clientOperation)
        {
        }        

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}
