using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class PrimitiveXmlElement : BaseXmlElement
    {
        public PrimitiveXmlElement(string name) : base(name)
        {
        }

        protected override XmlElementComponent CreateObject(Transform parent, BoundVariableCollection binder,
            LayoutInflater inflater, Dictionary<string, string> outerAttrs)
        {
            var gameObject = new GameObject();
            gameObject.transform.parent = parent;
            
            return gameObject.AddComponent<XmlElementComponent>();
        }
    }
}