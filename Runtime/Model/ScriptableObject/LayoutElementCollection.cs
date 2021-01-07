using System;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    [CreateAssetMenu]
    public class LayoutElementCollection : ElementCollection
    {
        public Entry[] individualElements = new Entry[0];
        public TextAsset[] collections = new TextAsset[0];

        protected override void RegisterElements()
        {
            foreach (var e in individualElements)
            {
                AddElement(new XmlSubLayoutElementInfo(e.name, e.layoutXml.text)
                    .AddGenericProperties());
            }

            foreach (var collection in collections)
            {
                foreach (var e in XmlUIUtils.GetElementsFromCollection(collection.text))
                {
                    AddElement(new XmlSubLayoutElementInfo(e.name, e.xmlContents)
                        .AddGenericProperties());
                }
            }
        }
        
        [Serializable]
        public struct Entry
        {
            public string name;
            public TextAsset layoutXml;
        }
    }
}