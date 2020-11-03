using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public static class XmlSchemaTypes
    {
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

        public static XmlTypeSchema GetEnumSchema<T>() where T : Enum
        {
            return new XmlTypeSchema
            {
                typeName = typeof(T).Name,
                baseType = "string",
                autocompleteValues = Enum.GetNames(typeof(T))
            };
        }
    }
}