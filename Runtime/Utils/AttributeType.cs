using System;
using System.Globalization;
using Facebook.Yoga;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ThreeDISevenZeroR.XmlUI
{
    public static class AttributeType
    {
        public static readonly TypeInfo<string> String = new TypeInfo<string> 
        {
            typeName = "String",
            baseType = "string",
            parser = ((string text, out string value) => { value = text; return true; })
        };
        
        public static readonly TypeInfo<int> Integer = new TypeInfo<int>
        {
            typeName = "Integer",
            baseType = "integer",
            parser = TryParseInt
        };
        
        public static readonly TypeInfo<float> Float = new TypeInfo<float>
        {
            typeName = "Float",
            baseType = "float",
            parser = TryParseFloat
        };
        
        public static readonly TypeInfo<bool> Boolean = new TypeInfo<bool>
        {
            typeName = "Boolean",
            baseType = "string",
            autocompleteValues = new[] { bool.FalseString, bool.TrueString },
            parser = bool.TryParse
        };
        
        public static readonly TypeInfo<Vector4> VectorValue = new TypeInfo<Vector4>
        {
            typeName = "String",
            baseType = "string",
            parser = TryParseVector4
        };
        
        public static readonly TypeInfo<YogaValue> YogaValue = new TypeInfo<YogaValue>
        {
            typeName = "YogaValue",
            baseType = "string",
            validationRegex = ".*",
            autocompleteValues = new[] { "Auto", "100%" },
            parser = TryParseYogaValue
        };

        public static readonly TypeInfo<Color> HtmlColor = new TypeInfo<Color>
        {
            typeName = "HtmlColor",
            baseType = "string",
            validationRegex = "#[0-9a-f]{3,8}",
            autocompleteValues = new[]
            {
                "red", 
                "cyan", 
                "blue", 
                "darkblue", 
                "lightblue", 
                "purple", 
                "yellow", 
                "lime",
                "fuchsia",
                "white",
                "silver",
                "grey", 
                "black", 
                "orange", 
                "brown", 
                "maroon", 
                "green", 
                "olive", 
                "navy", 
                "teal", 
                "aqua",
                "magenta"
            },
            parser = ColorUtility.TryParseHtmlString
        };

        public static TypeInfo<T> GetResourceType<T>() where T : Object
        {
            return new TypeInfo<T>
            {
                typeName = GetTypeName<T>(),
                baseType = "string",
                parser = (string text, out T result) =>
                {
                    result = Resources.Load<T>(text);

                    if (!result)
                        result = Resources.GetBuiltinResource<T>(text);
                
                    return result;
                }
            };
        }

        public static TypeInfo<T> GetEnumTypeInfo<T>() where T : struct, Enum
        {
            return new TypeInfo<T>
            {
                typeName = GetTypeName<T>(),
                baseType = "string",
                autocompleteValues = Enum.GetNames(typeof(T)),
                parser = Enum.TryParse
            };
        }

        private static string GetTypeName<T>()
        {
            return typeof(T).Namespace + "." + typeof(T).Name;
        }
        
        private static bool TryParseInt(string text, out int i) => 
            int.TryParse(text, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out i);

        private static bool TryParseFloat(string text, out float f) =>
            float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out f);

        private static bool TryParseYogaValue(string text, out YogaValue value)
        {
            if (text.Equals("auto", StringComparison.InvariantCultureIgnoreCase))
            {
                value = Facebook.Yoga.YogaValue.Auto();
                return true;
            }
            
            var isPercent = text.EndsWith("%");

            if (isPercent)
            {
                var numberValue = text.Substring(0, text.Length - 1);

                if (TryParseFloat(numberValue, out var percentValue))
                {
                    value = Facebook.Yoga.YogaValue.Percent(percentValue);
                    return true;
                }
            }
            else
            {
                if (TryParseFloat(text, out var sizeValue))
                {
                    value = Facebook.Yoga.YogaValue.Point(sizeValue);
                    return true;
                }
            }

            value = Facebook.Yoga.YogaValue.Undefined();
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