using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public class ConstantSetter<T> : IConstantSetter<T>
    {
        public string[] AttributeNames { get; }
        public bool IsSerializable { get; set; }
        public Action<XmlLayoutElement, T> Setter { get; private set; }

        public ConstantSetter(string[] attributeNames, Action<XmlLayoutElement, T> setter, bool isInstanceConstant)
        {
            this.Setter = setter;
            
            AttributeNames = attributeNames;
            IsSerializable = isInstanceConstant;
        }

        public void Apply(XmlLayoutElement element, T instance) => Setter(element, instance);
    }
}