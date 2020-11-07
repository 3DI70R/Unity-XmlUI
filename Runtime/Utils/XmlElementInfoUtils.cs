using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public static class XmlElementInfoUtils
    {
        public static BaseXmlElementInfo AddGenericProperties(this BaseXmlElementInfo element)
        {
            return element.AddOptionalComponent<CanvasGroup>(AttributeHandlers.CanvasGroup);
        }

        public static BaseXmlElementInfo AddOptionalBackground(this BaseXmlElementInfo element)
        {
            return element.AddOptionalComponent<Image>((g, c) => g.type = Image.Type.Sliced, 
                AttributeHandlers.Image,
                AttributeHandlers.Shadow,
                AttributeHandlers.Graphic);
        }
    }
}