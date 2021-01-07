using Facebook.Yoga;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public abstract class MeasureDirtyWatcher : MonoBehaviour
    {
        public XmlLayoutElement target;
        public UIBehaviour behaviour;

        private float prevMeasureWidth;
        private float prevMeasureHeight;
        protected ILayoutElement Layout => (ILayoutElement) behaviour;

        public virtual bool IsChanged()
        {
            var l = Layout;
            
            if (Mathf.Approximately(l.preferredWidth, prevMeasureWidth) && 
                Mathf.Approximately(l.preferredHeight, prevMeasureHeight))
                return false;
                
            prevMeasureWidth = l.preferredWidth;
            prevMeasureHeight = l.preferredHeight;
            return true;
        }
        
        public abstract YogaSize MeasureElement(YogaNode node, 
            float width, YogaMeasureMode widthmode, 
            float height, YogaMeasureMode heightmode);

        protected void MarkDirtyIfChanged()
        {
            if (IsChanged())
            {
                target.MarkDirty();
            }
        }
    }
}