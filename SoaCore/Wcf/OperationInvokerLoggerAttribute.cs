using System;
using System.ServiceModel.Description;

namespace SoaCore.Wcf
{
    public class OperationInvokerLoggerAttribute : Attribute, IOperationBehavior
    {
        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new OperationInvokerLogger(dispatchOperation.Invoker, operationDescription.Name);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }
}
