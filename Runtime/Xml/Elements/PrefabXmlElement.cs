using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class PrefabXmlElement : BaseXmlElement
    {
        private readonly XmlElementComponent prefabElement;
        
        public PrefabXmlElement(string name, XmlElementComponent prefab) : base(name)
        {
            prefabElement = prefab;
        }

        protected override XmlElementComponent CreateObject(Transform parent, BoundAttributeCollection binder, 
            LayoutInflater inflater, Dictionary<string, string> outerAttrs)
        {
            return Object.Instantiate(prefabElement, parent, false);
        }
    }
}