using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IXmlElementFactory : IXmlComponentFactory
    {
        bool SupportsChildren { get; }

        LayoutElement CreateElement(Transform root, BoundAttributeCollection collection, 
            LayoutInflater inflater, Dictionary<string, string> outerAttrs);
    }
}