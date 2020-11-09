using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public class ConstantSetter<T> : IConstantSetter<T>
    {
        public string[] AttributeNames { get; }
        public bool IsSerializable { get; set; }
        public Action<LayoutElement, T> Setter { get; private set; }

        public ConstantSetter(string[] attributeNames, Action<LayoutElement, T> setter, bool isInstanceConstant)
        {
            this.Setter = setter;
            
            AttributeNames = attributeNames;
            IsSerializable = isInstanceConstant;
        }

        public void Apply(LayoutElement element, T instance) => Setter(element, instance);
    }
}