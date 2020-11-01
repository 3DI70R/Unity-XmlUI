using System.Collections.Generic;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class LayoutXmlElement : BaseXmlElement
    {
        private readonly string documentXml;
        
        public LayoutXmlElement(string name, string documentXml) : base(name)
        {
            this.documentXml = documentXml;
        }

        protected override XmlElementComponent CreateObject(Transform parent, BoundVariableCollection binder,
            LayoutInflater inflater, Dictionary<string, string> outerAttrs)
        {
            var element = inflater.Inflate(parent, documentXml, null, outerAttrs);

            if (element.TryGetComponent<ComponentVariableBinder>(out var addedBinder))
                binder.AddChild(addedBinder);

            return element;
        }
    }
}