using System.Collections.Generic;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IAttributeHandler<in T>
    {
        IAttributeInfo[] Attributes { get; }

        IAttributeCollection<T> ParseAttributes(Dictionary<string, string> attributes);
    }
}