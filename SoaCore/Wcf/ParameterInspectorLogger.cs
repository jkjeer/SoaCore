using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using NLog;

namespace SoaCore.Wcf
{
    public class ParameterInspectorLogger : IParameterInspector
    {
        public object BeforeCall(string operationName, object[] inputs)
        {
            var operationContext = OperationContext.Current;
            var securityContext = ServiceSecurityContext.Current;

            //if (securityContext != null)
            try
            {
                var isAnonymous = securityContext.IsAnonymous;
                string arguments;
                string user;
                if (isAnonymous)
                {
                    user = "Anonymous";
                    arguments = "";
                }
                else
                {
                    user = securityContext.PrimaryIdentity.Name;
                    var sb = new StringBuilder();
                    foreach (var input in inputs)
                    {
                        if (input == null)
                        {
                            sb.Append("null");
                        }
                        else
                        {
                            sb.Append(input); 
                        }
                        sb.Append(", ");
                    }
                    arguments = sb.ToString().TrimEnd(' ').TrimEnd(',');
                }

                var logEvent = new LogEventInfo
                {
                    Level = LogLevel.Info,
                    LoggerName = GetType().FullName
                };
                logEvent.Properties.Add("EVENT_DATE", DateTime.Now);
                logEvent.Properties.Add("EVENT_TYPE", "INFO");
                logEvent.Properties.Add("USER_NAME", user);
                logEvent.Properties.Add("OPERATION_NAME", operationName);
                logEvent.Properties.Add("CONTRACT_NAME", operationContext.EndpointDispatcher.ContractName);
                logEvent.Properties.Add("LOG_INTERFACE", "IParameterInspector");
                logEvent.Properties.Add("ARGUMENTS", arguments);
                logEvent.Properties.Add("HOST_NAME", operationContext.InstanceContext.Host.Description.Name);
                logEvent.Properties.Add("SCHEME", operationContext.EndpointDispatcher.EndpointAddress.Uri.Scheme);
                logEvent.Properties.Add("PORT", operationContext.EndpointDispatcher.EndpointAddress.Uri.Port);
                logEvent.Properties.Add("SERVER_ADDRESS", operationContext.Channel.LocalAddress.Uri.ToString());
                logEvent.Properties.Add("SESSION_ID", operationContext.SessionId);
                logEvent.Properties.Add("ELAPSED_MS", 0);
                logEvent.Properties.Add("COMMENTS", "");
                RemoteEndpointMessageProperty x = operationContext.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logEvent.Properties.Add("REMOTE_IP", x?.Address ?? "");
                var logger = LogManager.GetCurrentClassLogger();
                logger.Log(logEvent);
            }
            catch (Exception e)
            {
                var eMsg = e.Message;                
            }
            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
        }
    }
}
