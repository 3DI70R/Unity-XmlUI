using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public class ConstantSetter<T> : IConstantSetter<T>
    {
        public string[] AttributeNames { get; }
        public Action<T> Setter { get; }

        public ConstantSetter(string[] attributeNames, Action<T> setter)
        {
            AttributeNames = attributeNames;
            Setter = setter;
        }

        public void Apply(T instance) => Setter(instance);
    }
}