using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public abstract class BaseXmlFactoryBuilder<B, F> where B : BaseXmlFactoryBuilder<B, F> 
        where F : BaseXmlFactoryBuilder<B, F>.ElementBuildFactory
    {
        public delegate void ObjectBuilder(XmlLayoutElement obj, BoundAttributeCollection binder);
        
        protected readonly List<Action<F, Dictionary<string, string>>> factoryBuildActions = 
            new List<Action<F, Dictionary<string, string>>>();
        protected readonly List<IAttributeInfo> attributes = new List<IAttributeInfo>();
        
        public string Name { get; }
        public virtual IAttributeInfo[] Attributes => attributes.ToArray();

        public BaseXmlFactoryBuilder(string name)
        {
            Name = name;
        }
        
        protected void AddAttributes<T>(params IAttributeHandler<T>[] handlers)
        {
            foreach (var h in handlers)
                attributes.AddRange(h.Attributes);
        }

        public B AddObjectHandlers(params IAttributeHandler<GameObject>[] handlers)
        {
            AddAttributes(handlers);
            factoryBuildActions.Add((obj, attrs) => obj.AddObjectAttributes(ParseAttributes(handlers, attrs)));
            return (B) this;
        }

        public B AddComponent<T>(params IAttributeHandler<T>[] handlers)
            where T : Component
        {
            return AddComponent(null, handlers);
        }

        public B AddComponent<T>(Action<T, XmlLayoutElement> configurator, params IAttributeHandler<T>[] handlers) 
            where T : Component
        {
            AddAttributes(handlers);
            factoryBuildActions.Add((obj, attrs) => obj.AddComponent<T>(configurator, ParseAttributes(handlers, attrs)));
            return (B) this;
        }
        
        public B AddOptionalComponent<T>(params IAttributeHandler<T>[] handlers)
            where T : Component
        {
            return AddOptionalComponent(null, handlers);
        }

        public B AddOptionalComponent<T>(Action<T, XmlLayoutElement> configurator, params IAttributeHandler<T>[] handlers)
            where T : Component
        {
            AddAttributes(handlers);
            factoryBuildActions.Add((obj, attrs) =>
            {
                var parsedAttrs = ParseAttributes(handlers, attrs);

                if (!parsedAttrs.IsEmpty())
                    obj.AddComponent<T>(configurator, parsedAttrs);
            });
            
            return (B) this;
        }

        protected abstract F CreateEmptyFactory();
        
        public virtual F BuildFactory(Dictionary<string, string> attrs)
        {
            var factory = CreateEmptyFactory();
            
            for (var i = 0; i < factoryBuildActions.Count; i++) 
                factoryBuildActions[i](factory, attrs);
            
            return factory;
        }

        private IAttributeCollection<T> ParseAttributes<T>(IAttributeHandler<T>[] handlers, Dictionary<string, string> attrs)
        {
            var parsedAttrs = handlers.Select(h => h.ParseAttributes(attrs)).ToArray();
            return new AttributeCollection<T>
            {
                SerializableConstants = parsedAttrs.SelectMany(a => a.SerializableConstants).ToArray(),
                NonSerializableConstants = parsedAttrs.SelectMany(a => a.NonSerializableConstants).ToArray(),
                Variables = parsedAttrs.SelectMany(a => a.Variables).ToArray()
            };
        }
        
        public class ElementBuildFactory : IXmlComponentFactory
        {
            protected readonly List<ObjectBuilder> objectBuildActions = new List<ObjectBuilder>();

            public void AddObjectAttributes(IAttributeCollection<GameObject> attrs = null)
            {
                if(attrs.IsEmpty())
                    return;
                
                objectBuildActions.Add((prefabObject, binder) =>
                {
                    foreach (var c in attrs.SerializableConstants)
                        c.Setter(prefabObject, prefabObject.gameObject);

                    binder.AddAttributes(prefabObject, prefabObject.gameObject, 
                        attrs.Variables, attrs.NonSerializableConstants);
                });
            }
            
            public void AddComponent<T>(Action<T, XmlLayoutElement> componentConfig, IAttributeCollection<T> attrs = null) where T : Component
            {
                objectBuildActions.Add((prefabObject, binder) =>
                {
                    if(!prefabObject.gameObject.TryGetComponent<T>(out var component))
                        component = prefabObject.gameObject.AddComponent<T>();

                    componentConfig?.Invoke(component, prefabObject);

                    if(attrs.IsEmpty())
                        return;
                    
                    foreach (var c in attrs.SerializableConstants) 
                        c.Setter(prefabObject, component);

                    binder.AddAttributes(prefabObject, component, 
                        attrs.Variables, attrs.NonSerializableConstants);
                });
            }

            public void BindAttrs(XmlLayoutElement element, 
                BoundAttributeCollection collection)
            {
                for (var i = 0; i < objectBuildActions.Count; i++) 
                    objectBuildActions[i](element, collection);
            }
        }
    }
}