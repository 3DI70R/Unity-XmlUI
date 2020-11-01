using System.Collections.Generic;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IAttributeHandler<in T>
    {
        IAttributeInfo[] Attributes { get; }
        
        bool HasRequiredAttributes(Dictionary<string, string> attributes);
        IAttributeCollection<T> ParseAttributes(Dictionary<string, string> attributes);
    }
}