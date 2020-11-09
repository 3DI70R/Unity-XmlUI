using System;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IChildElementAnimator
    {
        void AnimateAddition(LayoutElement element, RectTransform transform, CanvasGroup group, 
            Rect target, Action onComplete = null);
        
        void AnimateMove(LayoutElement element, RectTransform transform, CanvasGroup group, 
            Rect target, Action onComplete = null);
        
        void AnimateResize(LayoutElement element, RectTransform transform, CanvasGroup group, 
            Rect target, Action onComplete = null);
        
        void AnimateDeletion(LayoutElement element, RectTransform transform, CanvasGroup group,
            Rect target, Action onComplete = null);

        void FinishAnimation(LayoutElement element);
    }
}