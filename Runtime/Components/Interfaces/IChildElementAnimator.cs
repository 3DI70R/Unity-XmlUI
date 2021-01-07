using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IChildElementAnimator
    {
        void AnimateChildAppear(XmlLayoutElement element, CanvasGroup group, Rect target);
        void AnimateChildMove(XmlLayoutElement element, Vector2 position);
        void AnimateChildDisappear(XmlLayoutElement element, CanvasGroup group);
        void AnimateContainerResize(XmlLayoutElement element, Vector2 size);

        void StartAnimation();
        void FinishAnimation();
    }
}