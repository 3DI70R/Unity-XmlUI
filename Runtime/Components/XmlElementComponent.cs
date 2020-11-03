using System.Collections.Generic;
using Facebook.Yoga;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public class XmlElementComponent : UIBehaviour, ILayoutSelfController
    {
        public string Id
        {
            get => componentId;
            set => componentId = value;
        }
        
        public Transform ChildParent
        {
            get
            {
                if (childParentElement && this != childParentElement)
                    return childParentElement.ChildParent;

                return transform;
            }
        }

        public YogaNode Node
        {
            get
            {
                InitYogaNode();
                return yogaNode;
            }
        }

        [SerializeField]
        private XmlElementComponent childParentElement;

        private XmlElementComponent parentElement;
        
        [SerializeField]
        private string componentId;

        [SerializeField]
        private List<XmlElementComponent> childElements 
            = new List<XmlElementComponent>();
        
        [SerializeField]
        private UIBehaviour measuredElement;

        private YogaNode yogaNode;
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = (RectTransform) transform;
            rectTransform.anchorMin = new Vector2(0, 1); // Top left
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
        }
        
        public void SetMeasuredElement(UIBehaviour element)
        {
            this.measuredElement = element;
        }

        public void CalculateLayout()
        {
            if (Node.IsDirty)
            {
                Node.CalculateLayout();
                OnLayoutUpdated();
            }
        }

        private void OnLayoutUpdated()
        {
            if (yogaNode.HasNewLayout)
            {
                var xPosition = yogaNode.LayoutX;
                var yPosition = yogaNode.LayoutY;
                var width = yogaNode.LayoutWidth;
                var height = yogaNode.LayoutHeight;
            
                rectTransform.anchoredPosition = new Vector2(xPosition, -yPosition);
                rectTransform.sizeDelta = new Vector2(width, height);
            
                foreach (var element in childElements)
                    element.OnLayoutUpdated();
                
                yogaNode.MarkLayoutSeen();
            }
        }

        private void InitYogaNode()
        {
            if(yogaNode != null)
                return;
            
            yogaNode = new YogaNode();

            foreach (var element in childElements)
                yogaNode.AddChild(element.Node);
            
            if(measuredElement) 
                yogaNode.SetMeasureFunction(MeasureElement);
        }

        private YogaSize MeasureElement(YogaNode node, 
            float width, YogaMeasureMode widthmode, 
            float height, YogaMeasureMode heightmode)
        {
            if (measuredElement is Text text)
            {
                var horizontalSettings = text.GetGenerationSettings(Vector2.zero);
                var textWidth = text.cachedTextGeneratorForLayout.GetPreferredWidth(
                    text.text, horizontalSettings) / text.pixelsPerUnit;

                var resultWidth = widthmode != YogaMeasureMode.Undefined ? 
                    Mathf.Min(textWidth, width) : textWidth;

                var verticalSettings = text.GetGenerationSettings(new Vector2(resultWidth, 0));
                var textHeight = text.cachedTextGeneratorForLayout.GetPreferredHeight(
                    text.text, verticalSettings) / text.pixelsPerUnit;

                var resultHeight = heightmode != YogaMeasureMode.Undefined 
                    ? Mathf.Min(textHeight, height) 
                    : textHeight;
                
                return new YogaSize
                {
                    width = resultWidth,
                    height = resultHeight
                };
            }
            
            if (measuredElement is ILayoutElement behaviour)
            {
                var measuredRectTransform = (RectTransform) measuredElement.transform;
                var originalSize = measuredRectTransform.sizeDelta;
                var size = originalSize;
                
                behaviour.CalculateLayoutInputHorizontal();
                var preferredWidth = behaviour.preferredWidth;
                size.x = Mathf.Min(preferredWidth, width);
                measuredRectTransform.sizeDelta = size;
                
                behaviour.CalculateLayoutInputVertical();
                var preferredHeight = behaviour.preferredHeight;
                size.y = Mathf.Min(preferredHeight, height);
                measuredRectTransform.sizeDelta = originalSize;

                return new YogaSize
                {
                    width = size.x,
                    height = size.y
                };
            }
            
            return new YogaSize
            {
                width = width,
                height = height
            };
        }

        public void SetChildRoot(XmlElementComponent parentElement)
        {
            this.childParentElement = parentElement;
        }

        public void AddChild(XmlElementComponent element)
        {
            if (element.parentElement)
            {
                Debug.Log("Element already contain parent");
                return;
            }
            
            element.parentElement = this;
            
            Node.AddChild(element.Node);
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

        private void MarkYogaDirty()
        {
            if(yogaNode != null && yogaNode.IsMeasureDefined)
                yogaNode.MarkDirty();
        }

        public void SetLayoutHorizontal() => MarkYogaDirty();
        public void SetLayoutVertical() => MarkYogaDirty();
    }
}