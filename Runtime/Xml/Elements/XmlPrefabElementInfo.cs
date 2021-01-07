using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ThreeDISevenZeroR.XmlUI
{
    public class XmlPrefabElementInfo : BaseXmlElementInfo
    {
        private readonly XmlLayoutElement prefabElement;
        
        public XmlPrefabElementInfo(string name, XmlLayoutElement prefab) : base(name)
        {
            prefabElement = prefab;
        }

        protected override XmlLayoutElement CreateObject(Transform parent, BoundAttributeCollection binder, 
            LayoutInflater inflater, Dictionary<string, string> outerAttrs)
        {
            return Object.Instantiate(prefabElement, parent, false);
        }
    }
}