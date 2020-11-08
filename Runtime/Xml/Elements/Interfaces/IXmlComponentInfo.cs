using System.Collections.Generic;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IXmlComponentInfo
    {
        string Name { get; }
        IAttributeInfo[] Attributes { get; }
        
        IXmlComponentFactory CreateFactory(Dictionary<string, string> attrs);
    }
}