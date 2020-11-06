using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IChildElementAnimator
    {
        void AnimateAddition(RectTransform child, CanvasGroup group, Rect target);
        void AnimateMove(RectTransform child, CanvasGroup group, Rect target);
        void AnimateDeletion(RectTransform child, CanvasGroup group, Rect target);
    }
}