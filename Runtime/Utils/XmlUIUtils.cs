using System.Globalization;
using Facebook.Yoga;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public static class XmlUIUtils
    {
        public static bool IsEmpty<T>(this IAttributeCollection<T> c) => 
            c == null || c.SerializableConstants.Length == 0 && c.NonSerializableConstants.Length == 0 && c.Variables.Length == 0;

        public static AttributeHandler<T> AddStringProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, string> setter) => 
            handler.AddGenericProperty(name, XmlSchemaTypes.String, ((string text, out string value) => { value = text; return true; }), setter);

        public static AttributeHandler<T> AddBoolProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, bool> setter) =>
            handler.AddGenericProperty(name, XmlSchemaTypes.Boolean, bool.TryParse, setter);
        
        public static AttributeHandler<T> AddIntProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, int> setter) =>
            handler.AddGenericProperty(name, XmlSchemaTypes.Integer, TryParseInt, setter);
        
        public static AttributeHandler<T> AddFloatProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, float> setter) =>
            handler.AddGenericProperty(name, XmlSchemaTypes.Float, TryParseFloat, setter);
        
        public static AttributeHandler<T> AddColorProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, Color> setter) => 
            handler.AddGenericProperty(name, XmlSchemaTypes.String, ColorUtility.TryParseHtmlString, setter);

        public static AttributeHandler<T> AddVectorProperty<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, Vector4> setter) => 
            handler.AddGenericProperty(name, XmlSchemaTypes.String, TryParseVector4, setter);
        
        public static AttributeHandler<T> AddYogaValue<T>(
            this AttributeHandler<T> handler, string name, ValueSetterDelegate<T, YogaValue> setter) =>
            handler.AddGenericProperty(name, XmlSchemaTypes.String, TryParseYogaValue, setter);

        private static bool TryParseInt(string text, out int i) => 
            int.TryParse(text, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out i);

        private static bool TryParseFloat(string text, out float f) =>
            float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out f);

        private static bool TryParseYogaValue(string text, out YogaValue value)
        {
            if (text == "auto")
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
    }
}