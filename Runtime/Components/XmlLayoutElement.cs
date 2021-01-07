using System.Collections.Generic;
using Facebook.Yoga;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class XmlLayoutElement : MonoBehaviour
    {
        public Transform ChildParentTransform => Container.transform;
        
        public string Id
        {
            get => componentId;
            set => componentId = value;
        }

        public XmlLayoutElement Container
        {
            get => childContainer ? childContainer.Container : this;
            set => childContainer = value == this ? null : value;
        }

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

        public IReadOnlyList<XmlLayoutElement> Children => childElements;
        
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
        public YogaPositionType PositionType { get => Node.PositionType; set { Node.PositionType = value; } }

        public YogaWrap Wrap { get => Node.Wrap; set => Node.Wrap = value; }
        public float Flex { set => Node.Flex = value; }
        public float FlexGrow { get => Node.FlexGrow; set => Node.FlexGrow = value; }
        public float FlexShrink { get => Node.FlexShrink; set => Node.FlexShrink = value; }
        public YogaValue FlexBasis { get => Node.FlexBasis; set => Node.FlexBasis = value; }
        
        public bool UseMeasure
        {
            get => useDirtyWatcher;
            set 
            {
                useDirtyWatcher = value;
                UpdateDirtyWatcher();
            }
        }

        public Visibility Visibility
        {
            get => visibility;
            set
            {
                if (visibility == value) 
                    return;
                
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
        private XmlLayoutElement childContainer;

        [SerializeField, HideInInspector]
        private XmlLayoutElement parentElement;
        
        [SerializeField]
        private string componentId;

        [SerializeField, HideInInspector]
        private List<XmlLayoutElement> childElements 
            = new List<XmlLayoutElement>();

        [SerializeField, HideInInspector]
        private MeasureDirtyWatcher dirtyWatcher;

        [SerializeField]
        [HideInInspector]
        private Visibility visibility = Visibility.Visible;

        [SerializeField, 
         HideInInspector] 
        private bool useDirtyWatcher = true;

        private List<XmlLayoutElement> hidePendingElements = 
            new List<XmlLayoutElement>();

        private HashSet<XmlLayoutElement> showPendingElements 
            = new HashSet<XmlLayoutElement>();

        private bool autoUpdateLayout = true;
        private bool skipLayoutUpdater = true;
        private YogaNode yogaNode;
        private YogaUpdater yogaUpdater;
        private RectTransform rectTransformRef;
        private IChildElementAnimator childAnimator;
        
        private bool isFirstLayoutUpdateReceived;

        private bool UseAnimatedTransitions => childAnimator != null && isFirstLayoutUpdateReceived;
        
        private bool isGraphicCallbackRegistered;

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

        public void SetChildAnimator(IChildElementAnimator animator)
        {
            childAnimator = animator;
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

        public void OnCreatedFromXml() { }
        public void OnReturnedToPool() { }
        public void OnBroughtBackFromPool() { }
        
        public void ActivateHierarchy()
        {
            if(gameObject.activeSelf)
                return;
            
            gameObject.SetActive(true);
        }

        public void DeactivateHierarchy()
        {
            if(!gameObject.activeSelf)
                return;
            
            gameObject.SetActive(false);
        }

        private void UpdateVisibility()
        {
            var isVisible = visibility == Visibility.Visible;
            
            if (parentElement)
            {
                parentElement.OnChildVisibilityChanged(this, isVisible);
            }
            else
            {
                SetElementEnabled(this, isVisible);
            }

            Node.Display = visibility != Visibility.Gone ? YogaDisplay.Flex : YogaDisplay.None;
        }

        private void OnChildVisibilityChanged(XmlLayoutElement element, bool isVisible)
        {
            if (UseAnimatedTransitions)
            {
                if (isVisible)
                {
                    showPendingElements.Add(element);
                }
                else
                {
                    hidePendingElements.Add(element);
                }
            }
            else
            {
                SetElementEnabled(element, isVisible);
            }
        }

        private void SetElementEnabled(XmlLayoutElement element, bool isEnabled)
        {
            if (isEnabled)
            {
                element.ActivateHierarchy();
            }
            else
            {
                element.DeactivateHierarchy();
            }
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

        public void SetUseDirtyWatcher(MeasureDirtyWatcher watcher)
        {
            if(watcher == dirtyWatcher)
                return;
            
            Node.SetMeasureFunction(null);
            dirtyWatcher = watcher;
            UpdateDirtyWatcher();
        }

        private void UpdateDirtyWatcher()
        {
            if (dirtyWatcher && UseMeasure)
                Node.SetMeasureFunction(dirtyWatcher.MeasureElement);
        }
        
        public void MoveChild(XmlLayoutElement child, int newIndex)
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

        public void InsertChild(XmlLayoutElement child, int index)
        {
            if(childElements.Contains(child))
                return;
            
            child.SetParent(this);
            Node.Insert(index, child.Node);
            childElements.Insert(index, child);

            if (UseAnimatedTransitions)
            {
                child.DeactivateHierarchy();
                showPendingElements.Add(child);
            }
        }

        public void AddChild(XmlLayoutElement child)
        {
            if(childElements.Contains(child))
                return;
            
            child.SetParent(this);
            Node.AddChild(child.Node);
            childElements.Add(child);

            if (UseAnimatedTransitions)
            {
                child.DeactivateHierarchy();
                showPendingElements.Add(child);
            }
        }

        public void RemoveChild(XmlLayoutElement child)
        {
            if (childElements.Remove(child))
            {
                Node.RemoveChild(child.Node);

                if (UseAnimatedTransitions)
                {
                    hidePendingElements.Add(child);
                }
                else
                {
                    child.SetParent(null);
                }
            }
        }

        private void Start()
        {
            skipLayoutUpdater = false;
            UpdateLayoutUpdater();
        }

        private void OnEnable()
        {
            isFirstLayoutUpdateReceived = false;
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
            if(!Node.HasNewLayout || !isActiveAndEnabled)
                return;

            if (UseAnimatedTransitions)
            {
                ApplyLayoutWithAnimation();
            }
            else
            {
                ApplyLayoutWithoutAnimation();
            }

            isFirstLayoutUpdateReceived = true;
        }

        private void ApplyLayoutWithoutAnimation()
        {
            if (showPendingElements.Count > 0)
            {
                foreach (var addedElement in showPendingElements)
                    addedElement.ActivateHierarchy();
                
                showPendingElements.Clear();
            }

            if (hidePendingElements.Count > 0)
            {
                foreach (var element in hidePendingElements)
                    element.SetParent(null);
                
                hidePendingElements.Clear();
            }
            
            foreach (var element in childElements)
            {
                if (element.Node.HasNewLayout)
                {
                    element.RectTransform.anchoredPosition = GetElementPosition(element);;
                    element.OnLayoutUpdated();
                }
            }
            
            RectTransform.sizeDelta = new Vector2(Node.LayoutWidth, Node.LayoutHeight);
        }

        private void ApplyLayoutWithAnimation()
        {
            childAnimator.FinishAnimation();

            if (hidePendingElements.Count > 0)
            {
                foreach (var removedElement in hidePendingElements)
                    childAnimator.AnimateChildDisappear(removedElement, null);

                hidePendingElements.Clear();
            }
            
            var newSize = new Vector2(Node.LayoutWidth, Node.LayoutHeight);
            
            if (RectTransform.sizeDelta != newSize)
            {
                childAnimator.AnimateContainerResize(this, newSize);
            }

            foreach (var element in childElements)
            {
                var position = GetElementPosition(element);
                
                if (showPendingElements.Contains(element))
                {
                    var rect = new Rect(position.x, position.y, 
                        element.Node.LayoutWidth, element.Node.LayoutHeight);
                    
                    childAnimator.AnimateChildAppear(element, null, rect);
                }
                else if(element.Node.HasNewLayout)
                {
                    if (element.RectTransform.anchoredPosition != position)
                        childAnimator.AnimateChildMove(element, position);
                }

                element.OnLayoutUpdated();
            }

            showPendingElements.Clear();
            childAnimator.StartAnimation();
        }

        private Vector2 GetElementPosition(XmlLayoutElement element)
        {
            var pivot = element.RectTransform.pivot;
            return new Vector2(element.Node.LayoutX, -element.Node.LayoutY);
        }

        private void InitYogaNode()
        {
            if(yogaNode != null)
                return;

            yogaNode = new YogaNode();
            
            UpdateVisibility();
            UpdateDirtyWatcher();

            foreach (var element in childElements)
                yogaNode.AddChild(element.Node);
        }

        private void SetParent(XmlLayoutElement parent)
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
        
        public void MarkDirty()
        {
            if(yogaNode != null && yogaNode.IsMeasureDefined)
                yogaNode.MarkDirty();
        }

        private class YogaUpdater : MonoBehaviour
        {
            public XmlLayoutElement element;

            private void Start() => element.CalculateLayout();
            private void Update() => element.CalculateLayout();
        }
    }
}