using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ThreeDISevenZeroR.XmlUI
{
    [Serializable]
    public class BoundVariableCollection
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
        
        public void AddAttributes<T>(T component, IVariableBinder<T>[] binders) where T : Object
        {
            // TODO: unbind conflicting binders?
            
            var holder = ScriptableObject.CreateInstance<SettersReferenceHolder>();
            holder.setters = new Func<object, IVariableProvider, IBoundVariable>[binders.Length];

            for (var i = 0; i < binders.Length; i++)
            {
                var setter = binders[i];
                holder.setters[i] = (c, p) => setter.Bind((T) c, p);
            }

            setters.Add(new BoundReference
            {
                target = component,
                holder = holder
            });
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
                foreach (var s in r.holder.setters)
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
            public SettersReferenceHolder holder;
        }
        
        // Scriptable object which persists as reference during cloning phase
        // and keeps setters array, which cannot be serialized
        // You still cannot save them as prefab, though
        private class SettersReferenceHolder : ScriptableObject
        {
            public Func<object, IVariableProvider, IBoundVariable>[] setters;
        }
    }
}