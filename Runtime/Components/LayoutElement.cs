using System.Collections.Generic;
using Facebook.Yoga;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public class LayoutElement : MonoBehaviour
    {
        public string Id
        {
            get => componentId;
            set => componentId = value;
        }

        public LayoutElement Container
        {
            get => childContainer ? childContainer.Container : this;
            set => childContainer = value == this ? null : value;
        }

        public Transform ChildParentTransform => Container.transform;

        private YogaNode Node
        {
            get
            {
                InitYogaNode();
                return yogaNode;
            }
        }

        public bool AutoUpdateLayout
        {
            get => autoUpdateLayout;
            set
            {
                autoUpdateLayout = value;
                UpdateLayoutUpdater();
            }
        }
        
        public YogaValue OffsetLeft { get => Node.Left; set => Node.Left = value; }
        public YogaValue OffsetTop { get => Node.Top; set => Node.Top = value; }
        public YogaValue OffsetRight { get => Node.Right; set => Node.Right = value; }
        public YogaValue OffsetBottom { get => Node.Bottom; set => Node.Bottom = value; }
        public YogaValue OffsetStart { get => Node.Start; set => Node.Start = value; }
        public YogaValue OffsetEnd { get => Node.End; set => Node.End = value; }
        
        public YogaValue Margin { get => Node.Margin; set => Node.Margin = value; }
        public YogaValue MarginHorizontal { get => Node.MarginHorizontal; set => Node.MarginHorizontal = value; }
        public YogaValue MarginVertical { get => Node.MarginVertical; set => Node.MarginVertical = value; }
        public YogaValue MarginStart { get => Node.MarginStart; set => Node.MarginStart = value; }
        public YogaValue MarginEnd { get => Node.MarginEnd; set => Node.MarginEnd = value; }
        public YogaValue MarginTop { get => Node.MarginTop; set => Node.MarginTop = value; }
        public YogaValue MarginLeft { get => Node.MarginLeft; set => Node.MarginLeft = value; }
        public YogaValue MarginBottom { get => Node.MarginBottom; set => Node.MarginBottom = value; }
        public YogaValue MarginRight { get => Node.MarginRight; set => Node.MarginRight = value; }
        
        public YogaValue Padding { get => Node.Padding; set => Node.Padding = value; }
        public YogaValue PaddingHorizontal { get => Node.PaddingHorizontal; set => Node.PaddingHorizontal = value; }
        public YogaValue PaddingVertical { get => Node.PaddingVertical; set => Node.PaddingVertical = value; }
        public YogaValue PaddingStart { get => Node.PaddingStart; set => Node.PaddingStart = value; }
        public YogaValue PaddingEnd { get => Node.PaddingEnd; set => Node.PaddingEnd = value; }
        public YogaValue PaddingTop { get => Node.PaddingTop; set => Node.PaddingTop = value; }
        public YogaValue PaddingLeft { get => Node.PaddingLeft; set => Node.PaddingLeft = value; }
        public YogaValue PaddingBottom { get => Node.PaddingBottom; set => Node.PaddingBottom = value; }
        public YogaValue PaddingRight { get => Node.PaddingRight; set => Node.PaddingRight = value; }
        
        public YogaDirection StyleDirection { get => Node.StyleDirection; set => Node.StyleDirection = value; }
        public YogaFlexDirection FlexDirection { get => Node.FlexDirection; set => Node.FlexDirection = value; }
        public YogaJustify JustifyContent { get => Node.JustifyContent; set => Node.JustifyContent = value; }
        public YogaAlign AlignItems { get => Node.AlignItems; set => Node.AlignItems = value; }
        public YogaAlign AlignSelf { get => Node.AlignSelf; set => Node.AlignSelf = value; }
        public YogaAlign AlignContent { get => Node.AlignContent; set => Node.AlignContent = value; }
        public YogaPositionType PositionType { get => Node.PositionType; set => Node.PositionType = value; }
        public YogaWrap Wrap { get => Node.Wrap; set => Node.Wrap = value; }
        public float Flex { set => Node.Flex = value; }
        public float FlexGrow { get => Node.FlexGrow; set => Node.FlexGrow = value; }
        public float FlexShrink { get => Node.FlexShrink; set => Node.FlexShrink = value; }
        public YogaValue FlexBasis { get => Node.FlexBasis; set => Node.FlexBasis = value; }
        
        public Visibility Visibility
        {
            get => visibility;
            set
            {
                visibility = value;
                UpdateVisibility();
            }
        }
        
        public YogaValue Width { get => Node.Width; set => Node.Width = value; }
        public YogaValue Height { get => Node.Height; set => Node.Height = value; }
        public YogaValue MinWidth { get => Node.MinWidth; set => Node.MinWidth = value; }
        public YogaValue MinHeight { get => Node.MinHeight; set => Node.MinHeight = value; }
        public YogaValue MaxWidth { get => Node.MaxWidth; set => Node.MaxWidth = value; }
        public YogaValue MaxHeight { get => Node.MaxHeight; set => Node.MaxHeight = value; }
        public float AspectRatio { get => Node.AspectRatio; set => Node.AspectRatio = value; }
        public YogaOverflow Overflow { get => Node.Overflow; set => Node.Overflow = value; }

        [SerializeField]
        private LayoutElement childContainer;

        [SerializeField]
        [HideInInspector]
        private LayoutElement parentElement;
        
        [SerializeField]
        private string componentId;

        [SerializeField]
        [HideInInspector]
        private List<LayoutElement> childElements 
            = new List<LayoutElement>();
        
        [SerializeField]
        [HideInInspector]
        private UIBehaviour measuredElement;
        
        [SerializeField]
        [HideInInspector]
        private YogaMeasureDirtyWatcher measuredElementDirtyMarker;

        private bool autoUpdateLayout = true;
        private bool skipLayoutUpdater = true;
        private YogaNode yogaNode;
        private YogaUpdater yogaUpdater;
        private RectTransform rectTransformRef;

        private bool isHiearchyActive = true;
        private Visibility visibility = Visibility.Visible;

        public string GetYogaInfo()
        {
            return Node.Print(YogaPrintOptions.Style);
        }

        public RectTransform RectTransform
        {
            get
            {
                rectTransformRef = (RectTransform) transform;
                return rectTransformRef;
            }
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
        
        public void ActivateHierarchy()
        {
            gameObject.SetActive(true);
            isHiearchyActive = true;
        }

        public void DeactivateHierarchy()
        {
            gameObject.SetActive(false);
            isHiearchyActive = false;
        }
        
        private void UpdateVisibility()
        {
            var isActive = isHiearchyActive && visibility == Visibility.Visible;

            if(gameObject.activeSelf != isActive)
                gameObject.SetActive(isActive);

            Node.Display = visibility != Visibility.Gone ? YogaDisplay.Flex : YogaDisplay.None;
        }

        public void DetachFromParent()
        {
            SetParent(null);
        }
        
        public void CalculateLayout()
        {
            if (Node.IsDirty)
            {
                Node.CalculateLayout();
                OnLayoutUpdated();
            }
        }
        
        public void SetMeasuredElement(UIBehaviour element)
        {
            this.measuredElement = element;
        }
        
        public void MoveChild(LayoutElement child, int newIndex)
        {
            var childIndex = childElements.IndexOf(child);

            if (childIndex >= 0)
            {
                childElements.RemoveAt(childIndex);
                Node.RemoveAt(childIndex);
                
                childElements.Insert(newIndex, child);
                Node.Insert(newIndex, child.Node);
            }
        }

        public void InsertChild(LayoutElement child, int index)
        {
            if(childElements.Contains(child))
                return;
            
            child.SetParent(this);
            Node.Insert(index, child.Node);
            childElements.Insert(index, child);
        }

        public void AddChild(LayoutElement child)
        {
            if(childElements.Contains(child))
                return;
            
            child.SetParent(this);
            Node.AddChild(child.Node);
            childElements.Add(child);
        }

        public void RemoveChild(LayoutElement child)
        {
            if (childElements.Remove(child))
            {
                child.SetParent(null);
                Node.RemoveChild(child.Node);
            }
        }

        private void Start()
        {
            skipLayoutUpdater = false;
            UpdateLayoutUpdater();
        }

        private void UpdateLayoutUpdater()
        {
            if(skipLayoutUpdater)
                return;
            
            var shouldUpdate = autoUpdateLayout && !parentElement;

            if (shouldUpdate && !yogaUpdater)
            {
                yogaUpdater = gameObject.AddComponent<YogaUpdater>();
                yogaUpdater.element = this;
            }
            else if (!shouldUpdate && yogaUpdater)
            {
                DestroyImmediate(yogaUpdater);
            }
        }

        private void OnLayoutUpdated()
        {
            if (yogaNode.HasNewLayout)
            {
                var width = yogaNode.LayoutWidth;
                var height = yogaNode.LayoutHeight;
                
                if (parentElement)
                {
                    var xPosition = yogaNode.LayoutX;
                    var yPosition = yogaNode.LayoutY;
                    var pivot = RectTransform.pivot;
                    RectTransform.anchoredPosition = new Vector2(xPosition + width * pivot.x, -yPosition - height * pivot.y);
                }

                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

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

            if (measuredElement)
            {
                if (!measuredElementDirtyMarker)
                {
                    measuredElementDirtyMarker = gameObject.AddComponent<YogaMeasureDirtyWatcher>();
                    measuredElementDirtyMarker.element = this;
                }

                yogaNode.SetMeasureFunction(MeasureElement);
            }
        }

        private YogaSize MeasureElement(YogaNode node, 
            float width, YogaMeasureMode widthmode, 
            float height, YogaMeasureMode heightmode)
        {
            if (measuredElement is Text text)
            {
                if (string.IsNullOrWhiteSpace(text.text))
                    return new YogaSize { width = 0, height = 0 };
                
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

        private void SetParent(LayoutElement parent)
        {
            if (parentElement)
                parentElement.RemoveChild(this);

            if (parent)
            {
                if (RectTransform.parent != parent.ChildParentTransform)
                    RectTransform.parent.SetParent(parent.ChildParentTransform, false);
            }
            else
            {
                RectTransform.SetParent(null, false);
            }
            
            parentElement = parent;
            UpdateLayoutUpdater();
        }

        private void MarkYogaDirty()
        {
            if(yogaNode != null && yogaNode.IsMeasureDefined)
                yogaNode.MarkDirty();
        }

        private class YogaUpdater : MonoBehaviour
        {
            public LayoutElement element;

            private void Start() => element.CalculateLayout();
            private void Update() => element.CalculateLayout();
        }
        
        private class YogaMeasureDirtyWatcher : UIBehaviour, ILayoutSelfController
        {
            public LayoutElement element;
            private float prevWidth;
            private float prevHeight;

            private void MarkDirtyIfChanged()
            {
                if (element.measuredElement is ILayoutElement e 
                    && (e.preferredWidth != prevWidth || e.preferredHeight != prevHeight))
                {
                    prevWidth = e.preferredWidth;
                    prevHeight = e.preferredHeight;
                    
                    element.MarkYogaDirty();
                }
            }

            public void SetLayoutHorizontal() => MarkDirtyIfChanged();
            public void SetLayoutVertical() => MarkDirtyIfChanged();
        }
    }
}