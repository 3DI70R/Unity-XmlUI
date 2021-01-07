using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ThreeDISevenZeroR.XmlUI
{
    [Serializable]
    public class BoundAttributeCollection
    {
        [SerializeField]
        private List<BoundReference> setters = 
            new List<BoundReference>();
        
        [SerializeField]
        private List<ComponentVariableBinder> childBinders = 
            new List<ComponentVariableBinder>();
        
        private List<IBoundVariable> boundVariables = 
            new List<IBoundVariable>();

        public bool IsEmpty => setters.Count == 0;
        
        public void AddChild(ComponentVariableBinder binder)
        {
            childBinders.Add(binder);
        }
        
        public void AddAttributes<T>(XmlLayoutElement element, T component, 
            IVariableBinder<T>[] binders, 
            IConstantSetter<T>[] constants) where T : Object
        {
            if(binders.Length == 0 && constants.Length == 0)
                return;
            
            var holder = ScriptableObject.CreateInstance<VariableReferenceHolder>();
            holder.variables = new Func<object, IVariableProvider, IBoundVariable>[binders.Length];
            holder.constants = new Action<object>[constants.Length];

            for (var i = 0; i < binders.Length; i++)
            {
                var setter = binders[i];
                holder.variables[i] = (c, p) => setter.Bind(element, (T) c, p);
            }
            
            for (var i = 0; i < constants.Length; i++)
            {
                var setter = constants[i].Setter;
                holder.constants[i] = (c) => setter(element, (T) c);
            }

            setters.Add(new BoundReference
            {
                target = component,
                holder = holder
            });
        }

        public void ApplyConstants()
        {
            foreach (var r in setters)
            {
                foreach (var c in r.holder.constants)
                {
                    c(r.target);
                }
            }
        }
        
        public void UnbindFromProvider()
        {
            for(var i = 0; i < boundVariables.Count; i++)
                boundVariables[i].Unbind();
            
            boundVariables.Clear();
        }

        public void SetProvider(IVariableProvider provider)
        {
            foreach (var b in childBinders)
                b.SetVariableProvider(provider);
            
            BindToProvider(provider);
        }

        public void BindToProvider(IVariableProvider provider)
        {
            UnbindFromProvider();

            foreach (var r in setters)
            {
                foreach (var s in r.holder.variables)
                {
                    var bound = s(r.target, provider);
                    bound.Apply();
                    boundVariables.Add(bound);
                }
            }
        }
        
        [Serializable]
        private struct BoundReference
        {
            public Object target;
            
            [HideInInspector]
            public VariableReferenceHolder holder;
        }

        // Scriptable object which persists as reference during cloning phase
        // and keeps setters array, which cannot be serialized
        // You still cannot save them as prefab, though
        private class VariableReferenceHolder : ScriptableObject
        {
            public Action<object>[] constants;
            public Func<object, IVariableProvider, IBoundVariable>[] variables;
        }
    }
}