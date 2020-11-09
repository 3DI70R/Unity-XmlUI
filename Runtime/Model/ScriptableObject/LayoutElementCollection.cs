using System;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    [CreateAssetMenu]
    public class LayoutElementCollection : ElementCollection
    {
        [SerializeField]
        private LayoutElement[] elements = new LayoutElement[0];
        
        protected override void RegisterElements()
        {
            foreach (var e in elements)
            {
                var xmlElement = new XmlSubLayoutElementInfo(e.elementName, e.layout.text)
                    .AddGenericProperties();

                if (e.addButton)
                    xmlElement.AddComponent<Button>(AttributeHandlers.Selectable);

                if (e.addOptionalBackground)
                    xmlElement.AddOptionalBackground();
 
                AddElement(xmlElement);
            }
        }
        
        [Serializable]
        private struct LayoutElement
        {
            public string elementName;
            public TextAsset layout;
            
            public bool addButton;
            public bool addOptionalBackground;
        }
    }
}