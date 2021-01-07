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
                .AddComponent<Text>((c, e) =>
                    {
                        c.font = font;
                        c.raycastTarget = false;
                    },
                    AttributeHandlers.Text,
                    AttributeHandlers.Shadow,
                    AttributeHandlers.Graphic)
                .SetMeasuredComponent<Text, TextDirtyWatcher>());

            AddElement(new XmlPrimitiveElementInfo("Image")
                .AddGenericProperties()
                .AddComponent<Image>(
                    AttributeHandlers.Image,
                    AttributeHandlers.Shadow,
                    AttributeHandlers.Graphic));
            
            AddElement(new XmlPrimitiveElementInfo("RectMask")
                .AddGenericProperties()
                .AddComponent<RectMask2D>());
            
            AddComponent(new XmlComponentInfo("Mask")
                .AddComponent<Mask>(AttributeHandlers.Mask));
            
            AddComponent(new XmlComponentInfo("Button")
                .AddComponent<Button>(AttributeHandlers.Selectable));
            
            AddComponent(new XmlComponentInfo("InputField")
                .AddComponent<InputField>(AttributeHandlers.InputField, AttributeHandlers.Selectable));
            
            AddComponent(new XmlComponentInfo("Toggle")
                .AddComponent<Toggle>(AttributeHandlers.Toggle, AttributeHandlers.Selectable));
        }
    }
}