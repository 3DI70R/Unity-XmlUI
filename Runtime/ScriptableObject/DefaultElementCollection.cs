using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    [CreateAssetMenu]
    public class DefaultElementCollection : ElementCollection
    {
        public Font defaultTextFont;
        public TextAsset buttonLayout;

        protected override void RegisterElements()
        {
            var font = defaultTextFont 
                ? defaultTextFont 
                : Resources.GetBuiltinResource<Font>("Arial.ttf");
            
            AddElement(new PrimitiveXmlElement("Horizontal")
                .AddGenericProperties()
                .AddOptionalBackground()
                .AddComponent<HorizontalLayoutGroup>((g, c) =>
                    {
                        g.childForceExpandHeight = false;
                        g.childForceExpandWidth = false;
                    },
                    AttributeHandlers.LayoutGroup,
                    AttributeHandlers.HorizontalOrVerticalLayoutGroup));

            AddElement(new PrimitiveXmlElement("Vertical")
                .AddGenericProperties()
                .AddOptionalBackground()
                .AddComponent<VerticalLayoutGroup>((g, c) =>
                    {
                        g.childForceExpandHeight = false;
                        g.childForceExpandWidth = false;
                    },
                    AttributeHandlers.LayoutGroup,
                    AttributeHandlers.HorizontalOrVerticalLayoutGroup));
            
            AddElement(new PrimitiveXmlElement("Text")
                .AddGenericProperties()
                .AddComponent<Text>((g, c) => g.font = font,
                    AttributeHandlers.Text,
                    AttributeHandlers.Shadow,
                    AttributeHandlers.Graphic));

            AddElement(new PrimitiveXmlElement("Image")
                .AddGenericProperties()
                .AddComponent<Image>(
                    AttributeHandlers.Image,
                    AttributeHandlers.Shadow,
                    AttributeHandlers.Graphic));

            AddElement(new PrimitiveXmlElement("Space")
                .AddGenericProperties()
                .AddComponent<LayoutElement>((e, c) =>
                {
                    e.flexibleWidth = 1f;
                    e.flexibleHeight = 1f;
                }));

            if (buttonLayout)
            {
                AddElement(new LayoutXmlElement("Button", buttonLayout.text)
                    .AddGenericProperties()
                    .AddComponent<Button>(AttributeHandlers.Selectable));
            }
        }
    }
}