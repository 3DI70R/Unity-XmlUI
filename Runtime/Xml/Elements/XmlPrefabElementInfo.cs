using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ThreeDISevenZeroR.XmlUI
{
    public class XmlPrefabElementInfo : BaseXmlElementInfo
    {
        private readonly LayoutElement prefabElement;
        
        public XmlPrefabElementInfo(string name, LayoutElement prefab) : base(name)
        {
            prefabElement = prefab;
        }

        protected override LayoutElement CreateObject(Type elementType, Transform parent, 
            BoundAttributeCollection binder, LayoutInflater inflater, Dictionary<string, string> outerAttrs)
        {
            var instance = Object.Instantiate(prefabElement, parent, false);

            if (!instance.TryGetComponent(elementType, out var result))
            {
                throw new ArgumentException("Unable to create element from prefab, " +
                                            $"prefab doesnt contain component of type {elementType.Name}");
            }

            return instance;
        }
    }
}