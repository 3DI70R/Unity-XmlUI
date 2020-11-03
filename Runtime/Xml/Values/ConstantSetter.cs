using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public class ConstantSetter<T> : IConstantSetter<T>
    {
        public string[] AttributeNames { get; }
        public bool IsSerializable { get; set; }
        public Action<T> SetterDelegate => setter;

        private Action<T> setter;

        public ConstantSetter(string[] attributeNames, Action<T> setter, bool isInstanceConstant)
        {
            this.setter = setter;
            
            AttributeNames = attributeNames;
            IsSerializable = isInstanceConstant;
        }

        public void Apply(T instance) => setter(instance);
    }
}