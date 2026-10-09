using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace SoaCore.Logging
{
    public class ErrorLogEntry
    {
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string LoggingInterface { get; set; }
        [DataMember]
        public string OperationName { get; set; }
        [DataMember]
        public string ContractName { get; set; }
        [DataMember]
        public string Arguments { get; set; }
        [DataMember]
        public string HostName { get; set; }
        [DataMember]
        public string Scheme { get; set; }
        [DataMember]
        public int Port { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
        [DataMember]
        public string ServerAddress { get; set; }
        [DataMember]
        public string SessionId { get; set; }
        [DataMember]
        public long ElapsedMilliseconds { get; set; }
        [DataMember]
        public string ExceptionMessage { get; set; }

        public ErrorLogEntry()
        {
            
        }

        public ErrorLogEntry(Exception e, String reason)
        {
            OperationContext operationContext = OperationContext.Current;
            ServiceSecurityContext securityContext = ServiceSecurityContext.Current;

            UserName = securityContext.PrimaryIdentity.Name; 
            LoggingInterface = "ErrorLogEntry";
            OperationName = "";
            ContractName = operationContext.EndpointDispatcher.ContractName;
            Arguments = "";
            HostName = operationContext.InstanceContext.Host.Description.Name;
            Scheme = operationContext.EndpointDispatcher.EndpointAddress.Uri.Scheme;
            Port = operationContext.EndpointDispatcher.EndpointAddress.Uri.Port;
            ServerAddress = operationContext.Channel.LocalAddress.Uri.ToString();
            SessionId = operationContext.SessionId;
            TimeStamp = DateTime.Now;
            ElapsedMilliseconds = 0;

            string exceptionMessage = "MESSAGE:" +
                System.Environment.NewLine +
                reason +
                System.Environment.NewLine +
                "STACK TRACE:" +
                System.Environment.NewLine +
                e.StackTrace;
            ExceptionMessage = exceptionMessage;
        }

        public void persist()
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = ConfigurationManager.ConnectionStrings["SOA.LOG"].ConnectionString;
            sqlConnection.Open();
            try
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = sqlConnection;
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = "USP_INSERT_EVENT_LOG";

                sqlCommand.Parameters.Add(new SqlParameter("@EVENT_DATE", SqlDbType.DateTime));
                sqlCommand.Parameters.Add(new SqlParameter("@EVENT_TYPE", SqlDbType.VarChar, 8));
                sqlCommand.Parameters.Add(new SqlParameter("@USER_NAME", SqlDbType.VarChar, 50));
                sqlCommand.Parameters.Add(new SqlParameter("@OPERATION_NAME", SqlDbType.VarChar, 50));
                sqlCommand.Parameters.Add(new SqlParameter("@CONTRACT_NAME", SqlDbType.VarChar, 50));
                sqlCommand.Parameters.Add(new SqlParameter("@LOG_INTERFACE", SqlDbType.VarChar, 50));
                sqlCommand.Parameters.Add(new SqlParameter("@ARGUMENTS", SqlDbType.VarChar, 255));
                sqlCommand.Parameters.Add(new SqlParameter("@HOST_NAME", SqlDbType.VarChar, 50));
                sqlCommand.Parameters.Add(new SqlParameter("@SCHEME", SqlDbType.VarChar, 50));
                sqlCommand.Parameters.Add(new SqlParameter("@PORT", SqlDbType.Int));
                sqlCommand.Parameters.Add(new SqlParameter("@SERVER_ADDRESS", SqlDbType.VarChar, 255));
                sqlCommand.Parameters.Add(new SqlParameter("@SESSION_ID", SqlDbType.VarChar, 50));
                sqlCommand.Parameters.Add(new SqlParameter("@ELAPSED_MS", SqlDbType.Int));
                sqlCommand.Parameters.Add(new SqlParameter("@COMMENTS", SqlDbType.VarChar, 8000));

                sqlCommand.Parameters[0].Value = this.TimeStamp;
                sqlCommand.Parameters[1].Value = "ERROR";
                sqlCommand.Parameters[2].Value = this.UserName;
                sqlCommand.Parameters[3].Value = this.OperationName;
                sqlCommand.Parameters[4].Value = this.ContractName;
                sqlCommand.Parameters[5].Value = this.LoggingInterface;
                sqlCommand.Parameters[6].Value = this.Arguments;
                sqlCommand.Parameters[7].Value = this.HostName;
                sqlCommand.Parameters[8].Value = this.Scheme;
                sqlCommand.Parameters[9].Value = this.Port;
                sqlCommand.Parameters[10].Value = this.ServerAddress;
                sqlCommand.Parameters[11].Value = this.SessionId;
                sqlCommand.Parameters[12].Value = this.ElapsedMilliseconds;
                sqlCommand.Parameters[13].Value = this.ExceptionMessage;

                int rows = sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException err)
            {
                throw new ApplicationException(err.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }

        }

    }
}
