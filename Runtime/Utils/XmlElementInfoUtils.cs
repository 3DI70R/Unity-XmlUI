using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public static class XmlElementInfoUtils
    {
        public static BaseXmlElement AddGenericProperties(this BaseXmlElement element)
        {
            return element.AddObjectHandlers(
                    AttributeHandlers.GameObject,
                    AttributeHandlers.Position,
                    AttributeHandlers.Rotation,
                    AttributeHandlers.Scale)
                .AddOptionalComponent<CanvasGroup>(AttributeHandlers.CanvasGroup)
                .AddOptionalComponent<LayoutElement>(AttributeHandlers.LayoutElement)
                .AddOptionalComponent<ContentSizeFitter>(AttributeHandlers.ContentSizeFitter);
        }

        public static BaseXmlElement AddOptionalBackground(this BaseXmlElement element)
        {
            return element.AddOptionalComponent<Image>((g, c) => g.type = Image.Type.Sliced, 
                AttributeHandlers.Image,
                AttributeHandlers.Shadow,
                AttributeHandlers.Graphic);
        }
    }
}