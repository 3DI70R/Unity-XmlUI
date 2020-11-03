using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Component = UnityEngine.Component;

namespace ThreeDISevenZeroR.XmlUI
{
    public abstract class BaseXmlElement : IXmlElementInfo
    {
        private delegate void ObjectBuilder(XmlElementComponent obj, BoundAttributeCollection binder);
        
        protected readonly List<Action<ObjectBuildFactory, Dictionary<string, string>>> factoryBuildActions = 
            new List<Action<ObjectBuildFactory, Dictionary<string, string>>>();
        private readonly List<IAttributeInfo> attributes = new List<IAttributeInfo>();

        public string Name { get; }

        public IAttributeInfo[] Attributes => attributes.ToArray();
        
        private bool supportsChildren = true;

        public BaseXmlElement(string name)
        {
            Name = name;
            AddComponent(AttributeHandlers.XmlElement);
        }

        private void AddAttributes<T>(params IAttributeHandler<T>[] handlers)
        {
            foreach (var h in handlers)
                attributes.AddRange(h.Attributes);
        }

        public BaseXmlElement AddObjectHandlers(params IAttributeHandler<GameObject>[] handlers)
        {
            AddAttributes(handlers);
            factoryBuildActions.Add((obj, attrs) => obj.AddObjectAttributes(ParseAttributes(handlers, attrs)));
            return this;
        }

        public BaseXmlElement AddComponent<T>(params IAttributeHandler<T>[] handlers)
            where T : Component
        {
            return AddComponent(null, handlers);
        }

        public BaseXmlElement AddComponent<T>(Action<T, XmlElementComponent> configurator, params IAttributeHandler<T>[] handlers) 
            where T : Component
        {
            AddAttributes(handlers);
            factoryBuildActions.Add((obj, attrs) => obj.AddComponent<T>(configurator, ParseAttributes(handlers, attrs)));
            return this;
        }
        
        public BaseXmlElement AddOptionalComponent<T>(params IAttributeHandler<T>[] handlers)
            where T : Component
        {
            return AddOptionalComponent(null, handlers);
        }

        public BaseXmlElement AddOptionalComponent<T>(Action<T, XmlElementComponent> configurator, params IAttributeHandler<T>[] handlers)
            where T : Component
        {
            AddAttributes(handlers);
            factoryBuildActions.Add((obj, attrs) =>
            {
                var parsedAttrs = ParseAttributes(handlers, attrs);

                if (!parsedAttrs.IsEmpty())
                    obj.AddComponent<T>(configurator, parsedAttrs);
            });
            
            return this;
        }

        public BaseXmlElement SetMeasuredComponent<T>() where T : UIBehaviour
        {
            supportsChildren = false;
            factoryBuildActions.Add((obj, attrs) => { obj.SetMeasuredElement<T>(); });
            return this;
        }

        public IXmlElementFactory CreateFactory(Dictionary<string, string> attrs)
        {
            var factory = new ObjectBuildFactory(this, Name, supportsChildren);
            
            for (var i = 0; i < factoryBuildActions.Count; i++) 
                factoryBuildActions[i](factory, attrs);
            
            return factory;
        }

        protected abstract XmlElementComponent CreateObject(Transform parent, BoundAttributeCollection binder,
            LayoutInflater inflater, Dictionary<string, string> outerAttrs);

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

        protected class ObjectBuildFactory : IXmlElementFactory
        {
            private readonly List<ObjectBuilder> objectBuildActions = new List<ObjectBuilder>();

            private BaseXmlElement parentElement;
            private readonly string gameObjectName;
            
            public bool SupportsChildren { get; }

            public ObjectBuildFactory(BaseXmlElement parentElement, string gameObjectName, bool supportsChildren)
            {
                this.parentElement = parentElement;
                this.gameObjectName = gameObjectName;

                SupportsChildren = supportsChildren;
            }

            public void AddObjectAttributes(IAttributeCollection<GameObject> attrs = null)
            {
                if(attrs.IsEmpty())
                    return;
                
                objectBuildActions.Add((prefabObject, binder) =>
                {
                    foreach (var c in attrs.SerializableConstants)
                        c.Apply(prefabObject.gameObject);

                    binder.AddAttributes(prefabObject.gameObject, attrs.Variables, attrs.NonSerializableConstants);
                });
            }
            
            public void AddComponent<T>(Action<T, XmlElementComponent> componentConfig, IAttributeCollection<T> attrs = null) where T : Component
            {
                objectBuildActions.Add((prefabObject, binder) =>
                {
                    if(!prefabObject.gameObject.TryGetComponent<T>(out var component))
                        component = prefabObject.gameObject.AddComponent<T>();

                    componentConfig?.Invoke(component, prefabObject);

                    if(attrs.IsEmpty())
                        return;
                    
                    foreach (var c in attrs.SerializableConstants) 
                        c.Apply(component);

                    binder.AddAttributes(component, attrs.Variables, attrs.NonSerializableConstants);
                });
            }

            public void SetMeasuredElement<T>() where T : UIBehaviour
            {
                objectBuildActions.Add((prefabObject, binder) =>
                {
                    prefabObject.SetMeasuredElement(prefabObject.GetComponent<T>());
                });
            }

            public XmlElementComponent Create(Transform parent, BoundAttributeCollection binders,
                LayoutInflater inflater, Dictionary<string, string> outerAttrs)
            {
                var newObject = parentElement.CreateObject(parent, binders, inflater, outerAttrs);
                newObject.gameObject.name = gameObjectName;

                for (var i = 0; i < objectBuildActions.Count; i++) 
                    objectBuildActions[i](newObject, binders);

                return newObject;
            }
        }
    }
}