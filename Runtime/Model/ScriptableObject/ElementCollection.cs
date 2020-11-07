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
                if (!isInitialized || !Application.isPlaying)
                {
                    elementsList.Clear();
                    RegisterElements();
                }

                isInitialized = true;
                return elementsList.ToArray();
            }
        }

        private bool isInitialized;
        private readonly List<IXmlElementInfo> elementsList = 
            new List<IXmlElementInfo>();
        
        protected abstract void RegisterElements();

        protected void AddElement(IXmlElementInfo info)
        {
            elementsList.Add(info);
        }

        private void OnEnable()
        {
            isInitialized = false;
        }
    }
}

