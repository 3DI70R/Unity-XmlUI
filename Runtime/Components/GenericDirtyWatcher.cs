using Facebook.Yoga;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public class GenericDirtyWatcher : MeasureDirtyWatcher, ILayoutSelfController
    {
        public override YogaSize MeasureElement(YogaNode node, float width, YogaMeasureMode widthmode, float height,
            YogaMeasureMode heightmode)
        {
            var layout = Layout;
            var component = (Component) layout;
            var measuredRectTransform = (RectTransform) component.transform;
            var originalSize = measuredRectTransform.sizeDelta;
            var size = originalSize;
                
            layout.CalculateLayoutInputHorizontal();
            var preferredWidth = layout.preferredWidth;
            size.x = Mathf.Min(preferredWidth, width);
            measuredRectTransform.sizeDelta = size;
                
            layout.CalculateLayoutInputVertical();
            var preferredHeight = layout.preferredHeight;
            size.y = Mathf.Min(preferredHeight, height);
            measuredRectTransform.sizeDelta = originalSize;

            return new YogaSize
            {
                width = size.x,
                height = size.y
            };
        }
        
        public void SetLayoutHorizontal() => MarkDirtyIfChanged();
        public void SetLayoutVertical() { }
    }
}