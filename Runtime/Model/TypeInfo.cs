namespace ThreeDISevenZeroR.XmlUI
{
    public class TypeInfo
    {
        public string typeName;
        public string baseType;
        public string validationRegex;
        public string[] autocompleteValues;
    }
    
    public class TypeInfo<T> : TypeInfo
    {
        public StringParser<T> parser;
    }
}