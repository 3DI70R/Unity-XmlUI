using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public class XmlTypeSchema
    {
        public string typeName;
        public string baseType;
        public string validationRegex;
        public string[] autocompleteValues;
        
        public static readonly XmlTypeSchema String = new XmlTypeSchema
        {
            typeName = "String",
            baseType = "string"
        };
        
        public static readonly XmlTypeSchema Integer = new XmlTypeSchema
        {
            typeName = "Integer",
            baseType = "integer"
        };
        
        public static readonly XmlTypeSchema Float = new XmlTypeSchema
        {
            typeName = "Float",
            baseType = "float"
        };
        
        public static readonly XmlTypeSchema Boolean = new XmlTypeSchema
        {
            typeName = "Boolean",
            baseType = "boolean",
            autocompleteValues = new[] { bool.FalseString, bool.TrueString }
        };
        
        public static readonly XmlTypeSchema YogaValue = new XmlTypeSchema
        {
            typeName = "YogaValue",
            baseType = "string",
            validationRegex = ".*",
            autocompleteValues = new[] { "Auto", "100%" }
        };

        public static readonly XmlTypeSchema HtmlColor = new XmlTypeSchema
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
            }
        };

        public static XmlTypeSchema GetEnumSchema<T>() where T : Enum
        {
            return new XmlTypeSchema
            {
                typeName = typeof(T).Namespace + "." + typeof(T).Name,
                baseType = "string",
                autocompleteValues = Enum.GetNames(typeof(T))
            };
        }
    }
}