using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System.Xml;

namespace SoaCore.Wcf
{
    public class NetDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
    {
        private static readonly NetDataContractSerializer Serializer = new NetDataContractSerializer();

        public NetDataContractSerializerOperationBehavior(OperationDescription operationDescription) : base(operationDescription) { }

        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
        {
            return Serializer;
        }

        public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            return Serializer;
        }
    }
}
