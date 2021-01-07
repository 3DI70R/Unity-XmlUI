using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public delegate void BatchGetter<O, B>(O instance, B batch);
    public delegate void BatchSetter<O, B>(O instance, B batch);
    public delegate bool StringParser<P>(string text, out P value);
    public delegate void ValueSetterDelegate<T, P>(XmlLayoutElement element, T instance, P value);
    
    public static class XmlUIUtils
    {
        public const char AttrsDelimeter = ',';

        public const string AttrsReferenceAttributeId = "Attrs";
        public const string AttrsCollectionNameAttributeId = "Attrs.Name";
        public const string AttrsCollectionParentAttributeId = "Attrs.Parent";
        public const string ElementCollectionEntryNameAttributeId = "Name";

        public const string AttrsCollectionElementName = "AttributeCollection";
        public const string AttrsCollectionEntryName = "Attrs";
        public const string AttrsChildRootName = "ChildRoot";

        public const string ElementCollectionElementName = "ElementCollection";
        public const string ElementCollectionEntryName = "Element";
        
        public static BaseXmlElementInfo AddGenericProperties(this BaseXmlElementInfo element)
        {
            return element.AddOptionalComponent<CanvasGroup>(AttributeHandlers.CanvasGroup);
        }

        public static BaseXmlElementInfo AddOptionalBackground(this BaseXmlElementInfo element)
        {
            return element.AddOptionalComponent<Image>((g, c) =>
                {
                    g.type = Image.Type.Sliced;
                    g.raycastTarget = false;
                }, 
                AttributeHandlers.Image,
                AttributeHandlers.Shadow,
                AttributeHandlers.Graphic);
        }
        
        public static bool IsEmpty<T>(this IAttributeCollection<T> c) => 
            c == null || c.SerializableConstants.Length == 0 && c.NonSerializableConstants.Length == 0 && c.Variables.Length == 0;

        public static List<PlaceholderInfo> GetPlaceholderAttrs(string xmlLayout)
        {
            var result = new List<PlaceholderInfo>();

            void CollectPlaceholders(XmlElement element)
            {
                foreach (XmlAttribute attribute in element.Attributes)
                {
                    if (attribute.Value.StartsWith("@"))
                        result.Add(new PlaceholderInfo
                        {
                            placeholderName = attribute.Value.Substring(1),
                            attributeName = attribute.Name,
                            elementName = element.Name
                        });
                }

                foreach (XmlNode child in element.ChildNodes)
                {
                    if(child is XmlElement childElement)
                        CollectPlaceholders(childElement);
                }
            }

            var doc = new XmlDocument();
            doc.LoadXml(xmlLayout);
            CollectPlaceholders(doc.DocumentElement);
            return result;
        }

        public static List<CollectionElement> GetElementsFromCollection(string elementCollectionXml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(elementCollectionXml);
            
            var result = new List<CollectionElement>();

            if (doc.DocumentElement?.Name != ElementCollectionElementName)
            {
                Debug.LogWarning($"Invalid xml document root, <{ElementCollectionElementName}> is expected");
                return result;
            }

            foreach (XmlNode child in doc.DocumentElement.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element &&
                    child.Name == ElementCollectionEntryName)
                {
                    var element = (XmlElement) child;
                    var name = element.GetAttribute(ElementCollectionEntryNameAttributeId);
                    var xmlContents = child.InnerXml;
                        
                    result.Add(new CollectionElement
                    {
                        name = name,
                        xmlContents = xmlContents
                    });
                }
            }

            return result;
        }

        public struct CollectionElement
        {
            public string name;
            public string xmlContents;
        }

        public struct PlaceholderInfo
        {
            public string elementName;
            public string attributeName;
            public string placeholderName;
        }
    }
}