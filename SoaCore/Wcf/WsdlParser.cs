using System.Collections.Generic;
using System.Web.Services.Description;
using System.Xml;

namespace SoaCore.Wcf
{
    public class WsdlParser
    {
        public List<PortType> GetPortTypes(string wsdl)
        {
            var portTypes = new List<PortType>();
            XmlTextReader reader = new XmlTextReader(wsdl);
            var serviceDescription = ServiceDescription.Read(reader);
            foreach (PortType portType in serviceDescription.PortTypes)
            {
                portTypes.Add(portType);
            }
            return portTypes;
        } 
    }
}
