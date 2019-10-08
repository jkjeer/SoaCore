using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using NLog;
using SoaCore.Logging;

namespace SoaCore.Wcf
{
    public class OperationInvokerLogger : IOperationInvoker
    {
        private IOperationInvoker _defaultOperationInvoker;
        public string OperationName;
        public Logger logger = LogManager.GetCurrentClassLogger();

        public OperationInvokerLogger(IOperationInvoker operationInvoker, String operationName)
        {
            _defaultOperationInvoker = operationInvoker;
            OperationName = operationName;
        }

        public bool IsSynchronous
        {
            get { return _defaultOperationInvoker.IsSynchronous; }
        }

        public object[] AllocateInputs()
        {
            return _defaultOperationInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            Stopwatch stopwatch = null;
            Exception err = null;
            try
            {
                stopwatch = Stopwatch.StartNew();
                return _defaultOperationInvoker.Invoke(instance, inputs, out outputs);
            }
            catch (Exception e)
            {
                err = e;
                throw e;
            }
            finally
            {
                long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                OperationContext operationContext = OperationContext.Current;
                ServiceSecurityContext securityContext = ServiceSecurityContext.Current;

                string user = null;
                user = securityContext.PrimaryIdentity.Name;

                string exceptionMessage = "";
                if (err != null)
                {
                    exceptionMessage = "MESSAGE:" +
                        System.Environment.NewLine +
                        err.Message +
                        System.Environment.NewLine +
                        "STACK TRACE:" +
                        System.Environment.NewLine +
                        err.StackTrace;
                }

                string arguments = "";
                StringBuilder sb = new StringBuilder();
                foreach (object input in inputs)
                {
                    if (input == null)
                    {
                        sb.Append("null");
                    }
                    else
                    {
                        sb.Append(input.ToString());
                    }
                    sb.Append(", ");
                }
                if (sb.Length > 0)
                {
                    arguments = sb.ToString().TrimEnd(' ').TrimEnd(',');
                }

                LogEventInfo logEvent = new LogEventInfo();
                logEvent.Level = LogLevel.Info;
                logEvent.LoggerName = this.GetType().FullName;
                logEvent.Properties.Add("EVENT_DATE", DateTime.Now);
                logEvent.Properties.Add("EVENT_TYPE", "INFO");
                logEvent.Properties.Add("USER_NAME", user);
                logEvent.Properties.Add("OPERATION_NAME", this.OperationName);
                logEvent.Properties.Add("CONTRACT_NAME", operationContext.EndpointDispatcher.ContractName);
                logEvent.Properties.Add("LOG_INTERFACE", "IOperationInvoker");
                logEvent.Properties.Add("ARGUMENTS", arguments);
                logEvent.Properties.Add("HOST_NAME", operationContext.InstanceContext.Host.Description.Name);
                logEvent.Properties.Add("SCHEME", operationContext.EndpointDispatcher.EndpointAddress.Uri.Scheme);
                logEvent.Properties.Add("PORT", operationContext.EndpointDispatcher.EndpointAddress.Uri.Port);
                logEvent.Properties.Add("SERVER_ADDRESS", operationContext.Channel.LocalAddress.Uri.ToString());
                logEvent.Properties.Add("SESSION_ID", operationContext.SessionId);
                logEvent.Properties.Add("ELAPSED_MS", elapsedMilliseconds);
                logEvent.Properties.Add("COMMENTS", "");
                RemoteEndpointMessageProperty x = operationContext.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                logEvent.Properties.Add("REMOTE_IP", x?.Address ?? "");
                logger.Log(logEvent);
            }
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return _defaultOperationInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return _defaultOperationInvoker.InvokeEnd(instance, out outputs, result);
        }

    }
}
