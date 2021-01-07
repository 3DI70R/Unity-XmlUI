using System;
using Facebook.Yoga;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public class TextDirtyWatcher : MeasureDirtyWatcher
    {
        private string lastTextString;
        private bool isChangedFromCallback;

        private void OnEnable()
        {
            var text = (Text) behaviour;
            //text.RegisterDirtyLayoutCallback(OnDirtyCallback);
            MarkDirtyIfChanged();
        }

        private void OnDisable()
        {
            var text = (Text) behaviour;
            text.UnregisterDirtyLayoutCallback(OnDirtyCallback);
        }

        private void OnDirtyCallback()
        {
            isChangedFromCallback = true;
            MarkDirtyIfChanged();
        }

        public override bool IsChanged()
        {
            var text = (Text) behaviour;
            var newText = text.text;
            
            var isChanged = isChangedFromCallback || base.IsChanged() || newText != lastTextString;
            lastTextString = newText;
            isChangedFromCallback = false;
            
            return isChanged;
        }
        
        public override YogaSize MeasureElement(YogaNode node, float width, YogaMeasureMode widthmode, float height,
            YogaMeasureMode heightmode)
        {
            var text = (Text) behaviour;
            
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
    }
}