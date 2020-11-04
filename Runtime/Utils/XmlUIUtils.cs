using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Facebook.Yoga;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public delegate void BatchGetter<O, B>(O instance, B batch);
    public delegate void BatchSetter<O, B>(O instance, B batch);
    public delegate void ValueSetterDelegate<in T, in P>(T instance, P value);
    
    public static class XmlUIUtils
    {
        public static bool IsEmpty<T>(this IAttributeCollection<T> c) => 
            c == null || c.SerializableConstants.Length == 0 && c.NonSerializableConstants.Length == 0 && c.Variables.Length == 0;

        public static AttributeHandler<T> AddStringProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, string> setter) => 
            handler.AddGenericProperty(name, XmlTypeSchema.String, ((string text, out string value) => { value = text; return true; }), setter);

        public static AttributeHandler<T> AddBoolProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, bool> setter) =>
            handler.AddGenericProperty(name, XmlTypeSchema.Boolean, bool.TryParse, setter);
        
        public static AttributeHandler<T> AddIntProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, int> setter) =>
            handler.AddGenericProperty(name, XmlTypeSchema.Integer, TryParseInt, setter);
        
        public static AttributeHandler<T> AddFloatProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, float> setter) =>
            handler.AddGenericProperty(name, XmlTypeSchema.Float, TryParseFloat, setter);
        
        public static AttributeHandler<T> AddColorProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, Color> setter) => 
            handler.AddGenericProperty(name, XmlTypeSchema.HtmlColor, ColorUtility.TryParseHtmlString, setter);

        public static AttributeHandler<T> AddVectorProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, Vector4> setter) => 
            handler.AddGenericProperty(name, XmlTypeSchema.String, TryParseVector4, setter);
        
        public static AttributeHandler<T> AddYogaValue<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, YogaValue> setter) =>
            handler.AddGenericProperty(name, XmlTypeSchema.YogaValue, TryParseYogaValue, setter);

        private static bool TryParseInt(string text, out int i) => 
            int.TryParse(text, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out i);

        private static bool TryParseFloat(string text, out float f) =>
            float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out f);

        private static bool TryParseYogaValue(string text, out YogaValue value)
        {
            if (text.Equals("auto", StringComparison.InvariantCultureIgnoreCase))
            {
                value = YogaValue.Auto();
                return true;
            }
            
            var isPercent = text.EndsWith("%");

            if (isPercent)
            {
                var numberValue = text.Substring(0, text.Length - 1);

                if (TryParseFloat(numberValue, out var percentValue))
                {
                    value = YogaValue.Percent(percentValue);
                    return true;
                }
            }
            else
            {
                if (TryParseFloat(text, out var sizeValue))
                {
                    value = YogaValue.Point(sizeValue);
                    return true;
                }
            }

            value = YogaValue.Undefined();
            return false;
        }
        
        private static bool TryParseVector4(string text, out Vector4 vector)
        {
            var values = text.Split(',');
            var lastValue = 0f;
            vector = new Vector4();

            for (var i = 0; i < 4; i++)
            {
                if (values.Length > i)
                {
                    if (!TryParseFloat(values[i], out var newValue)) 
                        return false;

                    lastValue = newValue;
                }
                
                vector[i] = lastValue;
            }

            return true;
        }

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