using System;
using System.Collections.Generic;

namespace ThreeDISevenZeroR.XmlUI
{
    public class VariableProvider : IVariableProvider
    {
        private readonly Dictionary<Type, object> defaultValues 
            = new Dictionary<Type, object>();
        
        private readonly Dictionary<Type, Dictionary<string, object>> namedValues 
            = new Dictionary<Type, Dictionary<string, object>>();

        public static VariableProvider GetDefault()
        {
            var provider = new VariableProvider();
            
            return provider;
        }

        public VariableProvider SetDefaultValue<T>(T value)
        {
            defaultValues[typeof(T)] = value;
            return this;
        }
        
        public VariableProvider SetValue<T>(string key, T value)
        {
            var tType = typeof(T);
            
            if (!namedValues.TryGetValue(tType, out var values))
            {
                values = new Dictionary<string, object>();
                namedValues[tType] = values;
            }

            if (values.TryGetValue(key, out var existingVariable))
            {
                var variable = (VariableValue<T>) existingVariable;
                variable.Value = value;
            }
            else
            {
                values[key] = new VariableValue<T> { Value = value};
            }
            
            return this;
        }
        
        public IVariableValue<T> GetValue<T>(string key)
        {
            var tType = typeof(T);

            if (namedValues.TryGetValue(tType, out var values))
            {
                if (values.TryGetValue(key, out var value))
                    return (IVariableValue<T>) value;
            }

            if (defaultValues.TryGetValue(tType, out var defaultValue))
                return (IVariableValue<T>) defaultValue;

            return default;
        }

        private class VariableValue<T> : IVariableValue<T>
        {
            private T currentValue;
            
            public T Value
            {
                get => currentValue;
                set
                {
                    if (Equals(currentValue, value)) 
                        return;
                    
                    currentValue = value;
                    OnValueChanged?.Invoke(value);
                }
            }

            public event Action<T> OnValueChanged;
        }
    }
}