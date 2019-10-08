using System.ServiceModel;

namespace SoaCore.Wcf.IG
{
    public class IGServiceAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            return true;
        }
    }
}
