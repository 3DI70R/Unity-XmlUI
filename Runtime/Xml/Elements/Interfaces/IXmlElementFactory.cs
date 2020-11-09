using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IXmlElementFactory : IXmlComponentFactory
    {
        bool SupportsChildren { get; }

        LayoutElement CreateElement(Type elementType, Transform root,
            BoundAttributeCollection collection, LayoutInflater inflater,
            Dictionary<string, string> outerAttrs);
    }
}