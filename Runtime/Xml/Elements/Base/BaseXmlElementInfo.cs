using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ThreeDISevenZeroR.XmlUI
{
    public abstract class BaseXmlElementInfo : BaseXmlFactoryBuilder<BaseXmlElementInfo, BaseXmlElementInfo.ObjectBuildFactory>, IXmlElementInfo
    {
        public bool SupportsChildren { get; private set; } = true;
        
        public BaseXmlElementInfo(string name) : base(name)
        { 
            AddComponent(AttributeHandlers.LayoutElement);
        }

        protected override ObjectBuildFactory CreateEmptyFactory() => 
            new ObjectBuildFactory(this, Name, SupportsChildren);
        
        public IXmlElementFactory CreateFactory(Dictionary<string, string> attrs) => 
            base.BuildFactory(attrs);

        public BaseXmlElementInfo SetMeasuredComponent<T, W>() 
            where T : UIBehaviour
            where W : MeasureDirtyWatcher
        {
            SupportsChildren = false;
            factoryBuildActions.Add((obj, attrs) => { obj.SetMeasuredElement<T, W>(); });
            return this;
        }

        protected abstract XmlLayoutElement CreateObject(Transform parent, BoundAttributeCollection binder, 
            LayoutInflater inflater, Dictionary<string, string> outerAttrs);

        public class ObjectBuildFactory : ElementBuildFactory, IXmlElementFactory
        {
            private BaseXmlElementInfo parentElement;
            private readonly string gameObjectName;
            
            public bool SupportsChildren { get; }

            public ObjectBuildFactory(BaseXmlElementInfo parentElement, string gameObjectName, bool supportsChildren)
            {
                this.parentElement = parentElement;
                this.gameObjectName = gameObjectName;

                SupportsChildren = supportsChildren;
            }

            public void SetMeasuredElement<T, W>() 
                where T : UIBehaviour 
                where W : MeasureDirtyWatcher
            {
                objectBuildActions.Add((prefabObject, binder) =>
                {
                    var watcher = prefabObject.gameObject.AddComponent<W>();
                    watcher.behaviour = prefabObject.GetComponent<T>();
                    watcher.target = prefabObject;
                    prefabObject.SetUseDirtyWatcher(watcher);
                });
            }

            public XmlLayoutElement CreateElement(Transform parent, BoundAttributeCollection binders,
                LayoutInflater inflater, Dictionary<string, string> outerAttrs)
            {
                var element = parentElement.CreateObject(parent, binders, inflater, outerAttrs);
                element.RectTransform.anchorMin = new Vector2(0, 1); // Top left
                element.RectTransform.anchorMax = new Vector2(0, 1);
                element.RectTransform.pivot = new Vector2(0,1f); // Center
                element.gameObject.name = gameObjectName;

                return element;
            }

            public void BindAttrs(XmlLayoutElement element, BoundAttributeCollection collection)
            {
                for (var i = 0; i < objectBuildActions.Count; i++) 
                    objectBuildActions[i](element, collection);
            }
        }
    }
}