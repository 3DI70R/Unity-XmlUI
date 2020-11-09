using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IChildElementAnimator
    {
        void AnimateChildAppear(LayoutElement element, CanvasGroup group, Rect target);
        void AnimateChildMove(LayoutElement element, Vector2 position);
        void AnimateChildDisappear(LayoutElement element, CanvasGroup group);
        void AnimateContainerResize(LayoutElement element, Vector2 size);

        void StartAnimation();
        void FinishAnimation();
    }
}