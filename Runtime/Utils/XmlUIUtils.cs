using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public delegate void BatchGetter<O, B>(O instance, B batch);
    public delegate void BatchSetter<O, B>(O instance, B batch);
    public delegate bool StringParser<P>(string text, out P value);
    public delegate void ValueSetterDelegate<T, P>(LayoutElement element, T instance, P value);
    
    public static class XmlUIUtils
    {
        public static BaseXmlElementInfo AddGenericProperties(this BaseXmlElementInfo element)
        {
            return element.AddOptionalComponent<CanvasGroup>(AttributeHandlers.CanvasGroup);
        }

        public static BaseXmlElementInfo AddOptionalBackground(this BaseXmlElementInfo element)
        {
            return element.AddOptionalComponent<Image>((g, c) => g.type = Image.Type.Sliced, 
                AttributeHandlers.Image,
                AttributeHandlers.Shadow,
                AttributeHandlers.Graphic);
        }
        
        public static bool IsEmpty<T>(this IAttributeCollection<T> c) => 
            c == null || c.SerializableConstants.Length == 0 && c.NonSerializableConstants.Length == 0 && c.Variables.Length == 0;

        public static List<string> GetPlaceholderAttrs(string xmlLayout)
        {
            var result = new List<string>();

            void CollectPlaceholders(XmlElement element)
            {
                foreach (XmlAttribute attribute in element.Attributes)
                {
                    if (attribute.Value.StartsWith("@"))
                        result.Add(attribute.Value.Substring(1));
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
    }
}