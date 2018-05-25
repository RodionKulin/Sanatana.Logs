using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Sanatana.Logs.WCF.Behaviors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ErrorHandlerAttribute : Attribute, IErrorHandler, IServiceBehavior
    {        

        //methods
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                cd.ErrorHandlers.Add(new ErrorHandlerAttribute());
            }
        }

        public bool HandleError(Exception error)
        {
            string message = error.GetBaseException().Message;
            WcfLogsHub.WcfLogger.LogError(message, error);

            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            string errorId = Guid.NewGuid().ToString();
            error.Data.Add("ErrorId", errorId);

            var fe = new FaultException("An error occurred executing the WCF service call.", 
                new FaultCode(errorId));
            fault = Message.CreateMessage(version, fe.CreateMessageFault(), errorId);
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }                
    }
}
