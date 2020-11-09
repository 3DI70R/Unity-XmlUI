using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    [CreateAssetMenu]
    public class DefaultElementCollection : ElementCollection
    {
        public Font defaultTextFont;

        protected override void RegisterElements()
        {
            var font = defaultTextFont ? defaultTextFont : Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            AddElement(new XmlPrimitiveElementInfo("Panel")
                .AddGenericProperties()
                .AddOptionalBackground());

            AddElement(new XmlPrimitiveElementInfo("Text")
                .AddGenericProperties()
                .AddComponent<Text>((c, e) => c.font = font,
                    AttributeHandlers.Text,
                    AttributeHandlers.Shadow,
                    AttributeHandlers.Graphic)
                .SetMeasuredComponent<Text>());

            AddElement(new XmlPrimitiveElementInfo("Image")
                .AddGenericProperties()
                .AddComponent<Image>(
                    AttributeHandlers.Image,
                    AttributeHandlers.Shadow,
                    AttributeHandlers.Graphic));

            AddElement(new XmlPrimitiveElementInfo("Mask")
                .AddGenericProperties()
                .AddComponent<Image>(AttributeHandlers.Image, AttributeHandlers.Graphic)
                .AddComponent<Mask>(AttributeHandlers.Mask));
            
            AddElement(new XmlPrimitiveElementInfo("RectMask")
                .AddGenericProperties()
                .AddComponent<RectMask2D>());
        }
    }
}