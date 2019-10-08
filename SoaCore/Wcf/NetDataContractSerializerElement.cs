using System;
using System.ServiceModel.Configuration;

namespace SoaCore.Wcf
{
    public class NetDataContractSerializerElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(NetDataContractSerializerBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new NetDataContractSerializerBehavior();
        }
    }


}
