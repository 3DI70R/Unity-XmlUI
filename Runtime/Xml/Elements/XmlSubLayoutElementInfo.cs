using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class XmlSubLayoutElementInfo : BaseXmlElementInfo
    {
        private readonly string documentXml;
        private List<PlaceholderAttributeInfo> placeholderAttrs;

        public override IAttributeInfo[] Attributes
        {
            get
            {
                var originalAttrs = base.Attributes;

                if (placeholderAttrs == null)
                    placeholderAttrs = XmlUIUtils.GetPlaceholderAttrs(documentXml)
                        .Select(a => new PlaceholderAttributeInfo
                        {
                            Name = a.placeholderName,
                            ElementName = a.elementName,
                            ElementAttribute = a.attributeName
                        })
                        .ToList();

                return originalAttrs.Concat(placeholderAttrs).ToArray();
            }
        }

        public XmlSubLayoutElementInfo(string name, string documentXml) : base(name)
        {
            this.documentXml = documentXml;
        }

        protected override XmlLayoutElement CreateObject(Transform parent, BoundAttributeCollection binder,
            LayoutInflater inflater, Dictionary<string, string> outerAttrs)
        {
            var element = inflater.Inflate(parent, documentXml, null, outerAttrs);

            if (element.TryGetComponent<ComponentVariableBinder>(out var addedBinder))
                binder.AddChild(addedBinder);

            return element;
        }
        
        private class PlaceholderAttributeInfo : IPlaceholderAttributeInfo
        {
            public string Name { get; set; }
            public string ElementName { get; set; }
            public string ElementAttribute { get; set; }
            
            public TypeInfo Type => AttributeType.String;
        }
    }
}