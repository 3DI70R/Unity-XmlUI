using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class PrefabXmlElementInfo : BaseXmlElementInfo
    {
        private readonly LayoutElement prefabElement;
        
        public PrefabXmlElementInfo(string name, LayoutElement prefab) : base(name)
        {
            prefabElement = prefab;
        }

        protected override LayoutElement CreateObject(Transform parent, BoundAttributeCollection binder, 
            LayoutInflater inflater, Dictionary<string, string> outerAttrs)
        {
            return Object.Instantiate(prefabElement, parent, false);
        }
    }
}