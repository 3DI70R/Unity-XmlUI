using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IXmlElementFactory
    {
        bool SupportsChildren { get; }
        
        LayoutElement Create(Transform root, 
            BoundAttributeCollection collection,
            LayoutInflater inflater,
            Dictionary<string, string> outerAttrs);
    }
}