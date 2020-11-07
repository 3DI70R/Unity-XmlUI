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
            var font = defaultTextFont 
                ? defaultTextFont 
                : Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            AddElement(new PrimitiveXmlElementInfo("Panel")
                .AddOptionalBackground()
                .AddGenericProperties());

            AddElement(new PrimitiveXmlElementInfo("Text")
                .AddGenericProperties()
                .AddComponent<Text>((c, g) => c.font = font,
                    AttributeHandlers.Text,
                    AttributeHandlers.Shadow,
                    AttributeHandlers.Graphic)
                .SetMeasuredComponent<Text>());

            AddElement(new PrimitiveXmlElementInfo("Image")
                .AddGenericProperties()
                .AddComponent<Image>(
                    AttributeHandlers.Image,
                    AttributeHandlers.Shadow,
                    AttributeHandlers.Graphic));

            AddElement(new PrimitiveXmlElementInfo("Mask")
                .AddGenericProperties()
                .AddComponent<Image>(AttributeHandlers.Image, AttributeHandlers.Graphic)
                .AddComponent<Mask>(AttributeHandlers.Mask));
        }
    }
}