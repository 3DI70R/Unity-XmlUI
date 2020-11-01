using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IXmlElementFactory
    {
        XmlElementComponent Create(Transform root, 
            BoundVariableCollection collection,
            LayoutInflater inflater,
            Dictionary<string, string> outerAttrs);
    }
}