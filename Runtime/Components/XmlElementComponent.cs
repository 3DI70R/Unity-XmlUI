using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class XmlElementComponent : MonoBehaviour
    {
        public string Id
        {
            get => componentId;
            set => componentId = value;
        }
        
        [SerializeField]
        private XmlElementComponent childParentElement;
        
        [SerializeField]
        private string componentId;

        [SerializeField]
        private List<XmlElementComponent> childElements 
            = new List<XmlElementComponent>();

        public Transform ChildParent
        {
            get
            {
                if (childParentElement && this != childParentElement)
                    return childParentElement.ChildParent;

                return transform;
            }
        }

        public void SetChildRoot(XmlElementComponent parentElement)
        {
            this.childParentElement = parentElement;
        }

        public void AddChild(XmlElementComponent element)
        {
            childElements.Add(element);
        }

        public T FindComponentById<T>(string id)
        {
            if (Id == id && TryGetComponent<T>(out var result))
                return result;

            for (var i = 0; i < childElements.Count; i++)
            {
                var foundElement = childElements[i].FindComponentById<T>(id);

                if (foundElement != null)
                    return foundElement;
            }
            
            return default;
        }
    }
}