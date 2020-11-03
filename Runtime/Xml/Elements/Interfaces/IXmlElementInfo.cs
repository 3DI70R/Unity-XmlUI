using System.Collections.Generic;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IXmlElementInfo
    {
        string Name { get; }
        IAttributeInfo[] Attributes { get; }
        bool SupportsChildren { get; }
        
        IXmlElementFactory CreateFactory(Dictionary<string, string> attrs);
    }
}