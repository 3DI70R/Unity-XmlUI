using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class XmlPrimitiveElementInfo : BaseXmlElementInfo
    {
        public XmlPrimitiveElementInfo(string name) : base(name)
        {
        }

        protected override LayoutElement CreateObject(Transform parent, BoundAttributeCollection binder, 
            LayoutInflater inflater, Dictionary<string, string> outerAttrs)
        {
            var gameObject = new GameObject();
            
            if (parent is RectTransform)
                gameObject.AddComponent<RectTransform>();
            
            gameObject.transform.SetParent(parent, false);

            return gameObject.AddComponent<LayoutElement>();
        }
    }
}