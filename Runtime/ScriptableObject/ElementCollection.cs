using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public abstract class ElementCollection : ScriptableObject
    {
        public IXmlElementInfo[] Elements => elementsList.ToArray();

        private readonly List<IXmlElementInfo> elementsList = 
            new List<IXmlElementInfo>();
        
        protected abstract void RegisterElements();

        private void OnEnable()
        {
            RegisterElements();
        }

        protected void AddElement(IXmlElementInfo info)
        {
            elementsList.Add(info);
        }
    }
}

