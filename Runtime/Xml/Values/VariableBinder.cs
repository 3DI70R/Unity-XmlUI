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

        public IBoundVariable Bind(T instance, IVariableProvider provider)
        {
            var value = provider.GetValue<P>(VariableName);
            return new BoundVariable(instance, value, Setter);
        }
        
        private class BoundVariable: IBoundVariable
        {
            private T instance;
            private IVariableValue<P> value;
            private ValueSetterDelegate<T, P> setter;
            
            public event Action OnUpdated;

            public BoundVariable(T instance, IVariableValue<P> value, ValueSetterDelegate<T, P> setter)
            {
                this.instance = instance;
                this.value = value;
                this.setter = setter;

                value.OnValueChanged += ApplyChanged;
            }
            
            public void Apply()
            {
                setter(instance, value.Value);
            }

            public void Unbind()
            {
                value.OnValueChanged -= ApplyChanged;
            }

            private void ApplyChanged(P newValue)
            {
                setter(instance, newValue);
                OnUpdated?.Invoke();
            }
        }
    }
}