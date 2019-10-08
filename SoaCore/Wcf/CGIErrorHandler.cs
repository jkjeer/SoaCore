using System;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using NLog;

namespace SoaCore.Wcf
{
    public class CGIErrorHandler : IErrorHandler
    {
        private string username;
        Logger logger = LogManager.GetCurrentClassLogger();

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            username = ServiceSecurityContext.Current?.WindowsIdentity.Name;
            FaultException newEx = new FaultException();
            MessageFault msgFault = newEx.CreateMessageFault();
            fault = Message.CreateMessage(version, msgFault.Code, error.Message, newEx.Action);
        }

        public bool HandleError2(Exception error)
        {
            if (OperationContext.Current == null)
            {
                OperationContext.Current = (OperationContext)CallContext.LogicalGetData("WcfOperationContext");
            }
            OperationContext operationContext = OperationContext.Current;
            LogEventInfo logEvent = new LogEventInfo();
            logger.Error(error.Message + " " + error.StackTrace);
            try
            {
                logEvent.Level = LogLevel.Error;
                logEvent.LoggerName = this.GetType().FullName;
                logEvent.Properties.Add("EVENT_DATE", DateTime.Now);
                logEvent.Properties.Add("EVENT_TYPE", "ERROR");
                logEvent.Properties.Add("USER_NAME", username);
                logEvent.Properties.Add("OPERATION_NAME", "");
                try
                {
                    logEvent.Properties.Add("CONTRACT_NAME", operationContext?.EndpointDispatcher?.ContractName);
                }
                catch
                {
                    logEvent.Properties.Add("CONTRACT_NAME", "");
                }
                logEvent.Properties.Add("LOG_INTERFACE", "IErrorHandler");
                logEvent.Properties.Add("ARGUMENTS", "");
                try
                {
                    logEvent.Properties.Add("HOST_NAME", operationContext?.InstanceContext?.Host?.Description?.Name);
                }
                catch
                {
                    logEvent.Properties.Add("HOST_NAME", "");
                }
                logEvent.Properties.Add("SCHEME", operationContext?.EndpointDispatcher?.EndpointAddress?.Uri.Scheme);
                logEvent.Properties.Add("PORT", operationContext?.EndpointDispatcher?.EndpointAddress?.Uri.Port);
                logEvent.Properties.Add("SERVER_ADDRESS", operationContext?.Channel.LocalAddress.Uri.ToString());
                logEvent.Properties.Add("SESSION_ID", operationContext?.SessionId);
                logEvent.Properties.Add("ELAPSED_MS", 0);
                logEvent.Properties.Add("COMMENTS", error.Message);
                logger.Log(logEvent);
            }
            catch (Exception e)
            {
                logger.Error(e.Message + " " + e.StackTrace);
            }
            return true;
        }

        public bool HandleError(Exception error)
        {
            OperationContext operationContext = OperationContext.Current;
            LogEventInfo logEvent = new LogEventInfo();
            logger.Error(error.Message + " " + error.StackTrace);
            try
            {
                logEvent.Level = LogLevel.Error;
                logEvent.LoggerName = this.GetType().FullName;
                logEvent.Properties.Add("EVENT_DATE", DateTime.Now);
                logEvent.Properties.Add("EVENT_TYPE", "ERROR");
                logEvent.Properties.Add("USER_NAME", username);
                logEvent.Properties.Add("OPERATION_NAME", "");
                try
                {
                    logEvent.Properties.Add("CONTRACT_NAME", operationContext?.EndpointDispatcher?.ContractName);
                }
                catch
                {
                    logEvent.Properties.Add("CONTRACT_NAME", "");
                }
                logEvent.Properties.Add("LOG_INTERFACE", "IErrorHandler");
                logEvent.Properties.Add("ARGUMENTS", "");
                try
                {
                    logEvent.Properties.Add("HOST_NAME", operationContext?.InstanceContext?.Host?.Description?.Name);
                }
                catch
                {
                    logEvent.Properties.Add("HOST_NAME", "");
                }
                logEvent.Properties.Add("SCHEME", operationContext?.EndpointDispatcher?.EndpointAddress?.Uri.Scheme);
                logEvent.Properties.Add("PORT", operationContext?.EndpointDispatcher?.EndpointAddress?.Uri.Port);
                logEvent.Properties.Add("SERVER_ADDRESS", operationContext?.Channel.LocalAddress.Uri.ToString());
                logEvent.Properties.Add("SESSION_ID", operationContext?.SessionId);
                logEvent.Properties.Add("ELAPSED_MS", 0);
                logEvent.Properties.Add("COMMENTS", error.Message);
                logger.Log(logEvent);
            }
            catch (Exception e)
            {
                logger.Error(e.Message + " " + e.StackTrace);
            }
            return true;
        }
    }
}
