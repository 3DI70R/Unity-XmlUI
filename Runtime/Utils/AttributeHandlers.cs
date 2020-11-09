using Facebook.Yoga;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public static class AttributeHandlers
    {
        public static readonly IAttributeHandler<LayoutElement> LayoutElement = new AttributeHandler<LayoutElement>()
            .AddStringAttr("Id", (e, c, v) => c.Id = v)
            .AddEnumAttr<Visibility>("Visibility", (e, c, v) => c.Visibility = v)

            .AddYogaValueAttr("Left", (e, c, v) => c.OffsetLeft = v, false)
            .AddYogaValueAttr("Top", (e, c, v) => c.OffsetTop = v, false)
            .AddYogaValueAttr("Right", (e, c, v) => c.OffsetRight = v, false)
            .AddYogaValueAttr("Bottom", (e, c, v) => c.OffsetBottom = v, false)
            .AddYogaValueAttr("Start", (e, c, v) => c.OffsetStart = v, false)
            .AddYogaValueAttr("End", (e, c, v) => c.OffsetEnd = v, false)
            
            .AddYogaValueAttr("Margin", (e, c, v) => c.Margin = v, false)
            .AddYogaValueAttr("MarginHorizontal", (e, c, v) => c.MarginHorizontal = v, false)
            .AddYogaValueAttr("MarginVertical", (e, c, v) => c.MarginVertical = v, false)
            .AddYogaValueAttr("MarginStart", (e, c, v) => c.MarginStart = v, false)
            .AddYogaValueAttr("MarginEnd", (e, c, v) => c.MarginEnd = v, false)
            .AddYogaValueAttr("MarginTop", (e, c, v) => c.MarginTop = v, false)
            .AddYogaValueAttr("MarginLeft", (e, c, v) => c.MarginLeft = v, false)
            .AddYogaValueAttr("MarginBottom", (e, c, v) => c.MarginBottom = v, false)
            .AddYogaValueAttr("MarginRight", (e, c, v) => c.MarginRight = v, false)
            
            .AddYogaValueAttr("Padding", (e, c, v) => c.Padding = v, false)
            .AddYogaValueAttr("PaddingHorizontal", (e, c, v) => c.PaddingHorizontal = v, false)
            .AddYogaValueAttr("PaddingVertical", (e, c, v) => c.PaddingVertical = v, false)
            .AddYogaValueAttr("PaddingStart", (e, c, v) => c.PaddingStart = v, false)
            .AddYogaValueAttr("PaddingEnd", (e, c, v) => c.PaddingEnd = v, false)
            .AddYogaValueAttr("PaddingTop", (e, c, v) => c.PaddingTop = v, false)
            .AddYogaValueAttr("PaddingLeft", (e, c, v) => c.PaddingLeft = v, false)
            .AddYogaValueAttr("PaddingBottom", (e, c, v) => c.PaddingBottom = v, false)
            .AddYogaValueAttr("PaddingRight", (e, c, v) => c.PaddingRight = v, false)
            
            .AddEnumAttr<YogaDirection>("StyleDirection", (e, c, v) => c.StyleDirection = v, false)
            .AddEnumAttr<YogaFlexDirection>("FlexDirection", (e, c, v) => c.FlexDirection = v, false)
            .AddEnumAttr<YogaJustify>("JustifyContent", (e, c, v) => c.JustifyContent = v, false)
            .AddEnumAttr<YogaAlign>("AlignItems", (e, c, v) => c.AlignItems = v, false)
            .AddEnumAttr<YogaAlign>("AlignSelf", (e, c, v) => c.AlignSelf = v, false)
            .AddEnumAttr<YogaAlign>("AlignContent", (e, c, v) => c.AlignContent = v, false)
            .AddEnumAttr<YogaPositionType>("PositionType", (e, c, v) => c.PositionType = v, false)
            .AddEnumAttr<YogaWrap>("Wrap", (e, c, v) => c.Wrap = v, false)
            .AddFloatAttr("Flex", (e, c, v) => c.Flex = v, false)
            .AddFloatAttr("FlexGrow", (e, c, v) => c.FlexGrow = v, false)
            .AddFloatAttr("FlexShrink", (e, c, v) => c.FlexShrink = v, false)
            .AddYogaValueAttr("FlexBasis", (e, c, v) => c.FlexBasis = v, false)
            
            .AddYogaValueAttr("Width", (e, c, v) => c.Width = v, false)
            .AddYogaValueAttr("Height", (e, c, v) => c.Height = v, false)
            .AddYogaValueAttr("MinWidth", (e, c, v) => c.MinWidth = v, false)
            .AddYogaValueAttr("MinHeight", (e, c, v) => c.MinHeight = v, false)
            .AddYogaValueAttr("MaxWidth", (e, c, v) => c.MaxWidth = v, false)
            .AddYogaValueAttr("MaxHeight", (e, c, v) => c.MaxHeight = v, false)
            .AddFloatAttr("AspectRatio", (e, c, v) => c.AspectRatio = v, false)
            .AddEnumAttr<YogaOverflow>("Overflow", (e, c, v) => c.Overflow = v, false);
        
        // GameObject

        public static readonly IAttributeHandler<GameObject> Position = new AttributeHandler<VectorBatch>()
            .AddVectorAttr("Position", (e, c, v) => c.value = v)
            .AddFloatAttr("PositionX", (e, c, v) => c.value.x = v)
            .AddFloatAttr("PositionY", (e, c, v) => c.value.y = v)
            .AddFloatAttr("PositionZ", (e, c, v) => c.value.z = v)
            .AsBatchForObject<GameObject>(new VectorBatch(),
                getter: (c, b) => b.value = c.transform.localPosition,
                setter: (c, b) => c.transform.localPosition = b.value);
        
        public static readonly IAttributeHandler<GameObject> Rotation = new AttributeHandler<VectorBatch>()
            .AddVectorAttr("Rotation", (e, c, v) => c.value = v)
            .AddFloatAttr("RotationX", (e, c, v) => c.value.x = v)
            .AddFloatAttr("RotationY", (e, c, v) => c.value.y = v)
            .AddFloatAttr("RotationZ", (e, c, v) => c.value.z = v)
            .AsBatchForObject<GameObject>(new VectorBatch(),
                getter: (c, b) => b.value = c.transform.localEulerAngles,
                setter: (c, b) => c.transform.localEulerAngles = b.value);
        
        public static readonly IAttributeHandler<GameObject> Scale = new AttributeHandler<VectorBatch>()
            .AddVectorAttr("Scale", (e, c, v) => c.value = v)
            .AddFloatAttr("ScaleX", (e, c, v) => c.value.x = v)
            .AddFloatAttr("ScaleY", (e, c, v) => c.value.y = v)
            .AddFloatAttr("ScaleZ", (e, c, v) => c.value.z = v)
            .AsBatchForObject<GameObject>(new VectorBatch(),
                getter: (c, b) => b.value = c.transform.localScale,
                setter: (c, b) => c.transform.localScale = b.value);

        public static readonly IAttributeHandler<CanvasGroup> CanvasGroup = new AttributeHandler<CanvasGroup>()
            .AddFloatAttr("Alpha", (e, c, v) => c.alpha = v)
            .AddBoolAttr("BlocksRaycasts", (e, c, v) => c.blocksRaycasts = v)
            .AddBoolAttr("Interactable", (e, c, v) => c.interactable = v);
        
        // Interactable

        public static readonly IAttributeHandler<Selectable> Selectable = new AttributeHandler<SelectableBatch>()
            .AddStringAttr("TargetGraphicId", (e, c, v) => c.targetGraphic = e.FindComponentById<Graphic>(v))
            .AddEnumAttr<Selectable.Transition>("Transition", (e, c, v) => c.transition = v)
            .AddEnumAttr<Navigation.Mode>("NavigationMode", (e, c, v) => c.navigationMode = v)
            .AddColorAttr("NormalColor", (e, c, v) => c.colors.normalColor = v)
            .AddColorAttr("HighlightedColor", (e, c, v) => c.colors.highlightedColor = v)
            .AddColorAttr("PressedColor", (e, c, v) => c.colors.pressedColor = v)
            .AddColorAttr("SelectedColor", (e, c, v) => c.colors.selectedColor = v)
            .AddColorAttr("DisabledColor", (e, c, v) => c.colors.disabledColor = v)
            .AddFloatAttr("ColorMultiplier", (e, c, v) => c.colors.colorMultiplier = v)
            .AddFloatAttr("ColorFadeDuration", (e, c, v) => c.colors.fadeDuration = v)
            .AddResourceAttr<Sprite>("HighlightedSprite", (e, c, v) => c.spriteState.selectedSprite = v)
            .AddResourceAttr<Sprite>("PressedSprite", (e, c, v) => c.spriteState.pressedSprite = v)
            .AddResourceAttr<Sprite>("SelectedSprite", (e, c, v) => c.spriteState.selectedSprite = v)
            .AddResourceAttr<Sprite>("DisabledSprite", (e, c, v) => c.spriteState.disabledSprite = v)
            .AsBatchForObject<Selectable>(new SelectableBatch(), 
                getter: (o, d) =>
                {
                    d.targetGraphic = d.targetGraphic;
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
            .AddStringAttr("TextId", (e, c, v) => c.textComponent = e.FindComponentById<Text>(v))
            .AddStringAttr("PlaceholderId", (e, c, v) => c.placeholder = e.FindComponentById<Graphic>(v))
            .AddIntAttr("CharacterLimit", (e, c, v) => c.characterLimit = v)
            .AddEnumAttr<InputField.ContentType>("ContentType", (e, c, v) => c.contentType = v)
            .AddEnumAttr<InputField.LineType>("LineType", (e, c, v) => c.lineType = v)
            .AddEnumAttr<InputField.CharacterValidation>("CharacterValidation", (e, c, v) => c.characterValidation = v)
            
            .AddFloatAttr("CaretBlinkRate", (e, c, v) => c.caretBlinkRate = v)
            .AddIntAttr("CaretWidth", (e, c, v) => c.caretWidth = v)
            .AddBoolAttr("CustomCaretColor", (e, c, v) => c.customCaretColor = v)
            .AddColorAttr("CaretColor", (e, c, v) => c.caretColor = v)
            .AddColorAttr("SelectionColor", (e, c, v) => c.selectionColor = v)
            
            .AddBoolAttr("HideMobileInput", (e, c, v) => c.shouldHideMobileInput = v)
            .AddBoolAttr("ReadOnly", (e, c, v) => c.readOnly = v);

        // -- Graphic

        public static readonly IAttributeHandler<Graphic> Graphic = new AttributeHandler<Graphic>()
            .AddColorAttr("Color", (e, d, p) => d.color = p)
            .AddResourceAttr<Material>("Material", (e, d, p) => d.material = p);
        
        public static readonly IAttributeHandler<Mask> Mask = new AttributeHandler<Mask>()
            .AddBoolAttr("ShowMaskGraphic", (e, c, v) => c.showMaskGraphic = v);
        
        public static readonly IAttributeHandler<Graphic> Shadow = new AttributeHandler<EffectBatch>()
            .AddEnumAttr<ShadowType>("EffectType", (e, c, v) => c.type = v)
            .AddColorAttr("EffectColor", (e, c, v) => c.color = v)
            .AddVectorAttr("EffectOffset", (e, c, v) => c.offset = v)
            .AddBoolAttr("EffectUseGraphicAlpha", (e, c, v) => c.useGraphicAlpha = v)
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
            .AddResourceAttr<Sprite>("Sprite", (e, c, v) => c.sprite = v)
            .AddEnumAttr<Image.Type>("SpriteType", (e, c, v) => c.type = v);

        public static readonly IAttributeHandler<Text> Text = new AttributeHandler<Text>()
            .AddStringAttr("Text", (e, c, v) => c.text = v)
            .AddResourceAttr<Font>("Font", (e, c, v) => c.font = v)
            .AddEnumAttr<FontStyle>("FontStyle", (e, c, v) => c.fontStyle = v)
            .AddIntAttr("FontSize", (e, c, v) => c.fontSize = v)
            .AddIntAttr("LineSpacing", (e, c, v) => c.lineSpacing = v)
            .AddEnumAttr<TextAnchor>("Alignment", (e, c, v) => c.alignment = v)
            .AddBoolAttr("AlignByGeometry", (e, c, v) => c.alignByGeometry = v)
            .AddEnumAttr<HorizontalWrapMode>("HorizontalOverflow", (e, c, v) => c.horizontalOverflow = v)
            .AddEnumAttr<VerticalWrapMode>("VerticalOverflow", (e, c, v) => c.verticalOverflow = v)
            .AddBoolAttr("BestFit", (e, c, v) => c.resizeTextForBestFit = v)
            .AddIntAttr("MinSize", (e, c, v) => c.resizeTextMinSize = v)
            .AddIntAttr("MaxSize", (e, c, v) => c.resizeTextMaxSize = v);

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
            public Graphic targetGraphic;
            public Selectable.Transition transition;
            public ColorBlock colors;
            public SpriteState spriteState;
            public Navigation.Mode navigationMode;
        }
    }
}