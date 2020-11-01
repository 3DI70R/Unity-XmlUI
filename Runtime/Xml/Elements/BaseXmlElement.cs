using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Component = UnityEngine.Component;

namespace ThreeDISevenZeroR.XmlUI
{
    public abstract class BaseXmlElement : IXmlElementInfo
    {
        private delegate void ObjectBuilder(XmlElementComponent obj, BoundVariableCollection binder);
        
        protected readonly List<Action<ObjectBuildFactory, Dictionary<string, string>>> factoryBuildActions = 
            new List<Action<ObjectBuildFactory, Dictionary<string, string>>>();
        protected readonly List<IAttributeInfo> attributes = new List<IAttributeInfo>();

        public string Name { get; }

        public IAttributeInfo[] Attributes => attributes.ToArray();

        public BaseXmlElement(string name)
        {
            Name = name;
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

        public IXmlElementFactory CreateFactory(Dictionary<string, string> attrs)
        {
            var factory = new ObjectBuildFactory(this, Name);
            
            for (var i = 0; i < factoryBuildActions.Count; i++) 
                factoryBuildActions[i](factory, attrs);
            
            return factory;
        }

        protected abstract XmlElementComponent CreateObject(Transform parent, BoundVariableCollection binder,
            LayoutInflater inflater, Dictionary<string, string> outerAttrs);

        private IAttributeCollection<T> ParseAttributes<T>(IAttributeHandler<T>[] handlers, Dictionary<string, string> attrs)
        {
            var parsedAttrs = handlers.Select(h => h.ParseAttributes(attrs)).ToArray();
            return new AttributeCollection<T>
            {
                Constants = parsedAttrs.SelectMany(a => a.Constants).ToArray(),
                Variables = parsedAttrs.SelectMany(a => a.Variables).ToArray()
            };
        }

        protected class ObjectBuildFactory : IXmlElementFactory
        {
            private readonly List<ObjectBuilder> objectBuildActions = new List<ObjectBuilder>();

            private BaseXmlElement parentElement;
            private readonly string gameObjectName;

            public ObjectBuildFactory(BaseXmlElement parentElement, string gameObjectName)
            {
                this.parentElement = parentElement;
                this.gameObjectName = gameObjectName;
            }

            public void AddObjectAttributes(IAttributeCollection<GameObject> attrs = null)
            {
                if(attrs.IsEmpty())
                    return;
                
                objectBuildActions.Add((prefabObject, binder) =>
                {
                    foreach (var c in attrs.Constants)
                        c.Apply(prefabObject.gameObject);

                    if (attrs.Variables.Length > 0)
                        binder.AddAttributes(prefabObject.gameObject, attrs.Variables);
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
                    
                    foreach (var c in attrs.Constants) 
                        c.Apply(component);

                    if (attrs.Variables.Length > 0)
                        binder.AddAttributes(component, attrs.Variables);
                });
            }

            public XmlElementComponent Create(Transform parent, BoundVariableCollection binders,
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