using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public abstract class ElementCollection : ScriptableObject
    {
        public IXmlElementInfo[] Elements
        {
            get
            {
                Init();
                return elementsList.ToArray();
            }
        }

        public IXmlComponentInfo[] Components
        {
            get
            {
                Init();
                return componentsList.ToArray();
            }
        }

        private bool isInitialized;
        
        private readonly List<IXmlElementInfo> elementsList = 
            new List<IXmlElementInfo>();
        
        private readonly List<IXmlComponentInfo> componentsList 
            = new List<IXmlComponentInfo>();
        
        protected abstract void RegisterElements();

        protected void AddElement(IXmlElementInfo info)
        {
            elementsList.Add(info);
        }

        protected void AddComponent(IXmlComponentInfo info)
        {
            componentsList.Add(info);
        }

        private void Init()
        {
            if (!isInitialized || !Application.isPlaying)
            {
                elementsList.Clear();
                componentsList.Clear();
                RegisterElements();
            }

            isInitialized = true;
        }

        private void OnEnable()
        {
            isInitialized = false;
        }
    }
}

