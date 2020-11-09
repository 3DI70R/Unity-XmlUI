using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public class VariableBinder<T, P> : IVariableBinder<T>
    {
        public string[] AttributeNames { get; }
        public string VariableName { get; }
        
        public ValueSetterDelegate<T, P> Setter { get; }
        
        public VariableBinder(string[] attributeNames, string variableName, ValueSetterDelegate<T, P> setter)
        {
            AttributeNames = attributeNames;
            Setter = setter;
            VariableName = variableName;
        }

        public IBoundVariable Bind(LayoutElement element, T instance, IVariableProvider provider)
        {
            var value = provider.GetValue<P>(VariableName);
            return new BoundVariable(element, instance, value, Setter);
        }
        
        private class BoundVariable: IBoundVariable
        {
            private readonly LayoutElement element;
            private readonly T instance;
            private readonly IVariableValue<P> value;
            private readonly ValueSetterDelegate<T, P> setter;
            
            public event Action OnUpdated;

            public BoundVariable(LayoutElement element, T instance, IVariableValue<P> value, ValueSetterDelegate<T, P> setter)
            {
                this.element = element;
                this.instance = instance;
                this.value = value;
                this.setter = setter;

                value.OnValueChanged += ApplyChanged;
            }
            
            public void Apply()
            {
                setter(element, instance, value.Value);
            }

            public void Unbind()
            {
                value.OnValueChanged -= ApplyChanged;
            }

            private void ApplyChanged(P newValue)
            {
                setter(element, instance, newValue);
                OnUpdated?.Invoke();
            }
        }
    }
}