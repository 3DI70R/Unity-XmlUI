using Facebook.Yoga;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public static class AttributeHandlers
    {
        public static readonly IAttributeHandler<LayoutElement> LayoutElement = new AttributeHandler<LayoutElement>()
            .AddStringAttr("Id", (c, v) => c.Id = v)
            .AddEnumAttr<Visibility>("Visibility", (c, v) => c.Visibility = v)

            .AddYogaValueAttr("Left", (c, v) => c.OffsetLeft = v, false)
            .AddYogaValueAttr("Top", (c, v) => c.OffsetTop = v, false)
            .AddYogaValueAttr("Right", (c, v) => c.OffsetRight = v, false)
            .AddYogaValueAttr("Bottom", (c, v) => c.OffsetBottom = v, false)
            .AddYogaValueAttr("Start", (c, v) => c.OffsetStart = v, false)
            .AddYogaValueAttr("End", (c, v) => c.OffsetEnd = v, false)
            
            .AddYogaValueAttr("Margin", (c, v) => c.Margin = v, false)
            .AddYogaValueAttr("MarginHorizontal", (c, v) => c.MarginHorizontal = v, false)
            .AddYogaValueAttr("MarginVertical", (c, v) => c.MarginVertical = v, false)
            .AddYogaValueAttr("MarginStart", (c, v) => c.MarginStart = v, false)
            .AddYogaValueAttr("MarginEnd", (c, v) => c.MarginEnd = v, false)
            .AddYogaValueAttr("MarginTop", (c, v) => c.MarginTop = v, false)
            .AddYogaValueAttr("MarginLeft", (c, v) => c.MarginLeft = v, false)
            .AddYogaValueAttr("MarginBottom", (c, v) => c.MarginBottom = v, false)
            .AddYogaValueAttr("MarginRight", (c, v) => c.MarginRight = v, false)
            
            .AddYogaValueAttr("Padding", (c, v) => c.Padding = v, false)
            .AddYogaValueAttr("PaddingHorizontal", (c, v) => c.PaddingHorizontal = v, false)
            .AddYogaValueAttr("PaddingVertical", (c, v) => c.PaddingVertical = v, false)
            .AddYogaValueAttr("PaddingStart", (c, v) => c.PaddingStart = v, false)
            .AddYogaValueAttr("PaddingEnd", (c, v) => c.PaddingEnd = v, false)
            .AddYogaValueAttr("PaddingTop", (c, v) => c.PaddingTop = v, false)
            .AddYogaValueAttr("PaddingLeft", (c, v) => c.PaddingLeft = v, false)
            .AddYogaValueAttr("PaddingBottom", (c, v) => c.PaddingBottom = v, false)
            .AddYogaValueAttr("PaddingRight", (c, v) => c.PaddingRight = v, false)
            
            .AddEnumAttr<YogaDirection>("StyleDirection", (c, v) => c.StyleDirection = v, false)
            .AddEnumAttr<YogaFlexDirection>("FlexDirection", (c, v) => c.FlexDirection = v, false)
            .AddEnumAttr<YogaJustify>("JustifyContent", (c, v) => c.JustifyContent = v, false)
            .AddEnumAttr<YogaAlign>("AlignItems", (c, v) => c.AlignItems = v, false)
            .AddEnumAttr<YogaAlign>("AlignSelf", (c, v) => c.AlignSelf = v, false)
            .AddEnumAttr<YogaAlign>("AlignContent", (c, v) => c.AlignContent = v, false)
            .AddEnumAttr<YogaPositionType>("PositionType", (c, v) => c.PositionType = v, false)
            .AddEnumAttr<YogaWrap>("Wrap", (c, v) => c.Wrap = v, false)
            .AddFloatAttr("Flex", (c, v) => c.Flex = v, false)
            .AddFloatAttr("FlexGrow", (c, v) => c.FlexGrow = v, false)
            .AddFloatAttr("FlexShrink", (c, v) => c.FlexShrink = v, false)
            .AddYogaValueAttr("FlexBasis", (c, v) => c.FlexBasis = v, false)
            
            .AddYogaValueAttr("Width", (c, v) => c.Width = v, false)
            .AddYogaValueAttr("Height", (c, v) => c.Height = v, false)
            .AddYogaValueAttr("MinWidth", (c, v) => c.MinWidth = v, false)
            .AddYogaValueAttr("MinHeight", (c, v) => c.MinHeight = v, false)
            .AddYogaValueAttr("MaxWidth", (c, v) => c.MaxWidth = v, false)
            .AddYogaValueAttr("MaxHeight", (c, v) => c.MaxHeight = v, false)
            .AddFloatAttr("AspectRatio", (c, v) => c.AspectRatio = v, false)
            .AddEnumAttr<YogaOverflow>("Overflow", (c, v) => c.Overflow = v, false);
        
        // GameObject

        public static readonly IAttributeHandler<GameObject> Position = new AttributeHandler<VectorBatch>()
            .AddVectorAttr("Position", (c, v) => c.value = v)
            .AddFloatAttr("PositionX", (c, v) => c.value.x = v)
            .AddFloatAttr("PositionY", (c, v) => c.value.y = v)
            .AddFloatAttr("PositionZ", (c, v) => c.value.z = v)
            .AsBatchForObject<GameObject>(new VectorBatch(),
                getter: (c, b) => b.value = c.transform.localPosition,
                setter: (c, b) => c.transform.localPosition = b.value);
        
        public static readonly IAttributeHandler<GameObject> Rotation = new AttributeHandler<VectorBatch>()
            .AddVectorAttr("Rotation", (c, v) => c.value = v)
            .AddFloatAttr("RotationX", (c, v) => c.value.x = v)
            .AddFloatAttr("RotationY", (c, v) => c.value.y = v)
            .AddFloatAttr("RotationZ", (c, v) => c.value.z = v)
            .AsBatchForObject<GameObject>(new VectorBatch(),
                getter: (c, b) => b.value = c.transform.localEulerAngles,
                setter: (c, b) => c.transform.localEulerAngles = b.value);
        
        public static readonly IAttributeHandler<GameObject> Scale = new AttributeHandler<VectorBatch>()
            .AddVectorAttr("Scale", (c, v) => c.value = v)
            .AddFloatAttr("ScaleX", (c, v) => c.value.x = v)
            .AddFloatAttr("ScaleY", (c, v) => c.value.y = v)
            .AddFloatAttr("ScaleZ", (c, v) => c.value.z = v)
            .AsBatchForObject<GameObject>(new VectorBatch(),
                getter: (c, b) => b.value = c.transform.localScale,
                setter: (c, b) => c.transform.localScale = b.value);

        public static readonly IAttributeHandler<CanvasGroup> CanvasGroup = new AttributeHandler<CanvasGroup>()
            .AddFloatAttr("Alpha", (c, v) => c.alpha = v)
            .AddBoolAttr("BlocksRaycasts", (c, v) => c.blocksRaycasts = v)
            .AddBoolAttr("Interactable", (c, v) => c.interactable = v);
        
        // Interactable

        public static readonly IAttributeHandler<Selectable> Selectable = new AttributeHandler<SelectableBatch>()
            .AddEnumAttr<Selectable.Transition>("Transition", (c, v) => c.transition = v)
            .AddEnumAttr<Navigation.Mode>("NavigationMode", (c, v) => c.navigationMode = v)
            .AddColorAttr("NormalColor", (c, v) => c.colors.normalColor = v)
            .AddColorAttr("HighlightedColor", (c, v) => c.colors.highlightedColor = v)
            .AddColorAttr("PressedColor", (c, v) => c.colors.pressedColor = v)
            .AddColorAttr("SelectedColor", (c, v) => c.colors.selectedColor = v)
            .AddColorAttr("DisabledColor", (c, v) => c.colors.disabledColor = v)
            .AddFloatAttr("ColorMultiplier", (c, v) => c.colors.colorMultiplier = v)
            .AddFloatAttr("ColorFadeDuration", (c, v) => c.colors.fadeDuration = v)
            .AddResourceAttr<Sprite>("HighlightedSprite", (c, v) => c.spriteState.selectedSprite = v)
            .AddResourceAttr<Sprite>("PressedSprite", (c, v) => c.spriteState.pressedSprite = v)
            .AddResourceAttr<Sprite>("SelectedSprite", (c, v) => c.spriteState.selectedSprite = v)
            .AddResourceAttr<Sprite>("DisabledSprite", (c, v) => c.spriteState.disabledSprite = v)
            .AsBatchForObject<Selectable>(new SelectableBatch(), 
                getter: (o, d) =>
                {
                    d.transition = o.transition;
                    d.colors = o.colors;
                    d.spriteState = o.spriteState;
                    d.navigationMode = o.navigation.mode;
                },
                setter: (o, d) =>
                {
                    o.transition = d.transition;
                    o.colors = d.colors;
                    o.spriteState = d.spriteState;

                    var nav = o.navigation;
                    nav.mode = d.navigationMode;
                    o.navigation = nav;
                });

        public static readonly IAttributeHandler<InputField> InputField = new AttributeHandler<InputField>()
            .AddIntAttr("CharacterLimit", (c, v) => c.characterLimit = v)
            .AddEnumAttr<InputField.ContentType>("ContentType", (c, v) => c.contentType = v)
            .AddEnumAttr<InputField.LineType>("LineType", (c, v) => c.lineType = v)
            .AddFloatAttr("CaretBlinkRate", (c, v) => c.caretBlinkRate = v)
            .AddIntAttr("CaretWidth", (c, v) => c.caretWidth = v)
            .AddBoolAttr("CustomCaretColor", (c, v) => c.customCaretColor = v)
            .AddColorAttr("CaretColor", (c, v) => c.caretColor = v)
            .AddColorAttr("SelectionColor", (c, v) => c.selectionColor = v)
            .AddBoolAttr("HideMobileInput", (c, v) => c.shouldHideMobileInput = v)
            .AddBoolAttr("ReadOnly", (c, v) => c.readOnly = v);

        // -- Graphic

        public static readonly IAttributeHandler<Graphic> Graphic = new AttributeHandler<Graphic>()
            .AddColorAttr("Color", (d, p) => d.color = p)
            .AddResourceAttr<Material>("Material", (d, p) => d.material = p);
        
        public static readonly IAttributeHandler<Mask> Mask = new AttributeHandler<Mask>()
            .AddBoolAttr("ShowMaskGraphic", (c, v) => c.showMaskGraphic = v);
        
        public static readonly IAttributeHandler<Graphic> Shadow = new AttributeHandler<EffectBatch>()
            .AddEnumAttr<ShadowType>("EffectType", (c, v) => c.type = v)
            .AddColorAttr("EffectColor", (c, v) => c.color = v)
            .AddVectorAttr("EffectOffset", (c, v) => c.offset = v)
            .AddBoolAttr("EffectUseGraphicAlpha", (c, v) => c.useGraphicAlpha = v)
            .AsBatchForObject<Graphic>(new EffectBatch(), 
                getter: (o, b) =>
                {
                    if (o.TryGetComponent<Shadow>(out var shadow))
                    {
                        b.color = shadow.effectColor;
                        b.offset = shadow.effectDistance;
                        b.existingShadow = shadow;
                    }
                },
                setter: (o, b) =>
                {
                    Shadow effect;

                    switch (b.type)
                    {
                        case ShadowType.Shadow:
                            if(b.existingShadow is Outline)
                                Object.Destroy(b.existingShadow);

                            effect = b.existingShadow 
                                ? b.existingShadow 
                                : o.gameObject.AddComponent<Shadow>(); 
                            
                            break;
                        
                        case ShadowType.Outline:
                            if(b.existingShadow && !(b.existingShadow is Outline))
                                Object.Destroy(b.existingShadow);
                            
                            effect = b.existingShadow 
                                ? b.existingShadow 
                                : o.gameObject.AddComponent<Outline>();
                            break;
                        
                        default: return;
                    }

                    effect.effectColor = b.color;
                    effect.effectDistance = b.offset;
                    effect.useGraphicAlpha = b.useGraphicAlpha;
                });

        public static readonly IAttributeHandler<Image> Image = new AttributeHandler<Image>()
            .AddResourceAttr<Sprite>("Sprite", (c, v) => c.sprite = v)
            .AddEnumAttr<Image.Type>("SpriteType", (c, v) => c.type = v);

        public static readonly IAttributeHandler<Text> Text = new AttributeHandler<Text>()
            .AddStringAttr("Text", (c, v) => c.text = v)
            .AddResourceAttr<Font>("Font", (c, v) => c.font = v)
            .AddEnumAttr<FontStyle>("FontStyle", (c, v) => c.fontStyle = v)
            .AddIntAttr("FontSize", (c, v) => c.fontSize = v)
            .AddIntAttr("LineSpacing", (c, v) => c.lineSpacing = v)
            .AddEnumAttr<TextAnchor>("Alignment", (c, v) => c.alignment = v)
            .AddBoolAttr("AlignByGeometry", (c, v) => c.alignByGeometry = v)
            .AddEnumAttr<HorizontalWrapMode>("HorizontalOverflow", (c, v) => c.horizontalOverflow = v)
            .AddEnumAttr<VerticalWrapMode>("VerticalOverflow", (c, v) => c.verticalOverflow = v)
            .AddBoolAttr("BestFit", (c, v) => c.resizeTextForBestFit = v)
            .AddIntAttr("MinSize", (c, v) => c.resizeTextMinSize = v)
            .AddIntAttr("MaxSize", (c, v) => c.resizeTextMaxSize = v);

        private enum ShadowType
        {
            Disabled,
            Shadow,
            Outline
        }
        
        private class VectorBatch
        {
            public Vector3 value;
        }

        private class EffectBatch
        {
            public Shadow existingShadow;
            public ShadowType type;
            public Vector2 offset;
            public Color color;
            public bool useGraphicAlpha;
        }

        private class SelectableBatch
        {
            public Selectable.Transition transition;
            public ColorBlock colors;
            public SpriteState spriteState;
            public Navigation.Mode navigationMode;
        }
    }
}