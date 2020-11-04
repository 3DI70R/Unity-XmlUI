using Facebook.Yoga;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public static class AttributeHandlers
    {
        public static readonly IAttributeHandler<LayoutElement> LayoutElement = new AttributeHandler<LayoutElement>()
            .AddStringProperty("Id", (c, v) => c.Id = v);
        
        public static readonly IAttributeHandler<LayoutElement> LayoutElementYogaParams =
            new AttributeHandler<LayoutElement>()
                .SetConstantsSerializable(false)
                .AddYogaValue("Left", (c, v) => c.OffsetLeft = v)
                .AddYogaValue("Top", (c, v) => c.OffsetTop = v)
                .AddYogaValue("Right", (c, v) => c.OffsetRight = v)
                .AddYogaValue("Bottom", (c, v) => c.OffsetBottom = v)
                .AddYogaValue("Start", (c, v) => c.OffsetStart = v)
                .AddYogaValue("End", (c, v) => c.OffsetEnd = v)
                
                .AddYogaValue("Margin", (c, v) => c.Margin = v)
                .AddYogaValue("MarginHorizontal", (c, v) => c.MarginHorizontal = v)
                .AddYogaValue("MarginVertical", (c, v) => c.MarginVertical = v)
                .AddYogaValue("MarginStart", (c, v) => c.MarginStart = v)
                .AddYogaValue("MarginEnd", (c, v) => c.MarginEnd = v)
                .AddYogaValue("MarginTop", (c, v) => c.MarginTop = v)
                .AddYogaValue("MarginLeft", (c, v) => c.MarginLeft = v)
                .AddYogaValue("MarginBottom", (c, v) => c.MarginBottom = v)
                .AddYogaValue("MarginRight", (c, v) => c.MarginRight = v)
                
                .AddYogaValue("Padding", (c, v) => c.Padding = v)
                .AddYogaValue("PaddingHorizontal", (c, v) => c.PaddingHorizontal = v)
                .AddYogaValue("PaddingVertical", (c, v) => c.PaddingVertical = v)
                .AddYogaValue("PaddingStart", (c, v) => c.PaddingStart = v)
                .AddYogaValue("PaddingEnd", (c, v) => c.PaddingEnd = v)
                .AddYogaValue("PaddingTop", (c, v) => c.PaddingTop = v)
                .AddYogaValue("PaddingLeft", (c, v) => c.PaddingLeft = v)
                .AddYogaValue("PaddingBottom", (c, v) => c.PaddingBottom = v)
                .AddYogaValue("PaddingRight", (c, v) => c.PaddingRight = v)
                
                .AddEnumProperty<YogaDirection>("StyleDirection", (c, v) => c.StyleDirection = v)
                .AddEnumProperty<YogaFlexDirection>("FlexDirection", (c, v) => c.FlexDirection = v)
                .AddEnumProperty<YogaJustify>("JustifyContent", (c, v) => c.JustifyContent = v)
                .AddEnumProperty<YogaAlign>("AlignItems", (c, v) => c.AlignItems = v)
                .AddEnumProperty<YogaAlign>("AlignSelf", (c, v) => c.AlignSelf = v)
                .AddEnumProperty<YogaAlign>("AlignContent", (c, v) => c.AlignContent = v)
                .AddEnumProperty<YogaPositionType>("PositionType", (c, v) => c.PositionType = v)
                .AddEnumProperty<YogaWrap>("Wrap", (c, v) => c.Wrap = v)
                .AddFloatProperty("Flex", (c, v) => c.Flex = v)
                .AddFloatProperty("FlexGrow", (c, v) => c.FlexGrow = v)
                .AddFloatProperty("FlexShrink", (c, v) => c.FlexShrink = v)
                .AddYogaValue("FlexBasis", (c, v) => c.FlexBasis = v)
                
                .AddYogaValue("Width", (c, v) => c.Width = v)
                .AddYogaValue("Height", (c, v) => c.Height = v)
                .AddYogaValue("MinWidth", (c, v) => c.MinWidth = v)
                .AddYogaValue("MinHeight", (c, v) => c.MinHeight = v)
                .AddYogaValue("MaxWidth", (c, v) => c.MaxWidth = v)
                .AddYogaValue("MaxHeight", (c, v) => c.MaxHeight = v)
                .AddFloatProperty("AspectRatio", (c, v) => c.AspectRatio = v)
                .AddEnumProperty<YogaOverflow>("Overflow", (c, v) => c.Overflow = v);
        
        // GameObject
        
        public static readonly IAttributeHandler<GameObject> GameObject = new AttributeHandler<GameObject>()
            .AddBoolProperty("IsActive", (c, v) => c.SetActive(v));

        public static readonly IAttributeHandler<GameObject> Position = new AttributeHandler<VectorBatch>()
            .AddVectorProperty("Position", (c, v) => c.value = v)
            .AddFloatProperty("PositionX", (c, v) => c.value.x = v)
            .AddFloatProperty("PositionY", (c, v) => c.value.y = v)
            .AddFloatProperty("PositionZ", (c, v) => c.value.z = v)
            .AsBatchForObject<GameObject>(new VectorBatch(),
                getter: (c, b) => b.value = c.transform.localPosition,
                setter: (c, b) => c.transform.localPosition = b.value);
        
        public static readonly IAttributeHandler<GameObject> Rotation = new AttributeHandler<VectorBatch>()
            .AddVectorProperty("Rotation", (c, v) => c.value = v)
            .AddFloatProperty("RotationX", (c, v) => c.value.x = v)
            .AddFloatProperty("RotationY", (c, v) => c.value.y = v)
            .AddFloatProperty("RotationZ", (c, v) => c.value.z = v)
            .AsBatchForObject<GameObject>(new VectorBatch(),
                getter: (c, b) => b.value = c.transform.localEulerAngles,
                setter: (c, b) => c.transform.localEulerAngles = b.value);
        
        public static readonly IAttributeHandler<GameObject> Scale = new AttributeHandler<VectorBatch>()
            .AddVectorProperty("Scale", (c, v) => c.value = v)
            .AddFloatProperty("ScaleX", (c, v) => c.value.x = v)
            .AddFloatProperty("ScaleY", (c, v) => c.value.y = v)
            .AddFloatProperty("ScaleZ", (c, v) => c.value.z = v)
            .AsBatchForObject<GameObject>(new VectorBatch(),
                getter: (c, b) => b.value = c.transform.localScale,
                setter: (c, b) => c.transform.localScale = b.value);

        public static readonly IAttributeHandler<CanvasGroup> CanvasGroup = new AttributeHandler<CanvasGroup>()
            .AddFloatProperty("Alpha", (c, v) => c.alpha = v)
            .AddBoolProperty("BlocksRaycasts", (c, v) => c.blocksRaycasts = v)
            .AddBoolProperty("Interactable", (c, v) => c.interactable = v);
        
        // Interactable

        public static readonly IAttributeHandler<Selectable> Selectable = new AttributeHandler<SelectableBatch>()
            .AddEnumProperty<Selectable.Transition>("Transition", (c, v) => c.transition = v)
            .AddEnumProperty<Navigation.Mode>("NavigationMode", (c, v) => c.navigationMode = v)
            .AddColorProperty("NormalColor", (c, v) => c.colors.normalColor = v)
            .AddColorProperty("HighlightedColor", (c, v) => c.colors.highlightedColor = v)
            .AddColorProperty("PressedColor", (c, v) => c.colors.pressedColor = v)
            .AddColorProperty("SelectedColor", (c, v) => c.colors.selectedColor = v)
            .AddColorProperty("DisabledColor", (c, v) => c.colors.disabledColor = v)
            .AddFloatProperty("ColorMultiplier", (c, v) => c.colors.colorMultiplier = v)
            .AddFloatProperty("ColorFadeDuration", (c, v) => c.colors.fadeDuration = v)
            .AddResourceProperty<Sprite>("HighlightedSprite", (c, v) => c.spriteState.selectedSprite = v)
            .AddResourceProperty<Sprite>("PressedSprite", (c, v) => c.spriteState.pressedSprite = v)
            .AddResourceProperty<Sprite>("SelectedSprite", (c, v) => c.spriteState.selectedSprite = v)
            .AddResourceProperty<Sprite>("DisabledSprite", (c, v) => c.spriteState.disabledSprite = v)
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
            .AddIntProperty("CharacterLimit", (c, v) => c.characterLimit = v)
            .AddEnumProperty<InputField.ContentType>("ContentType", (c, v) => c.contentType = v)
            .AddEnumProperty<InputField.LineType>("LineType", (c, v) => c.lineType = v)
            .AddFloatProperty("CaretBlinkRate", (c, v) => c.caretBlinkRate = v)
            .AddIntProperty("CaretWidth", (c, v) => c.caretWidth = v)
            .AddBoolProperty("CustomCaretColor", (c, v) => c.customCaretColor = v)
            .AddColorProperty("CaretColor", (c, v) => c.caretColor = v)
            .AddColorProperty("SelectionColor", (c, v) => c.selectionColor = v)
            .AddBoolProperty("HideMobileInput", (c, v) => c.shouldHideMobileInput = v)
            .AddBoolProperty("ReadOnly", (c, v) => c.readOnly = v);

        // -- Graphic

        public static readonly IAttributeHandler<Graphic> Graphic = new AttributeHandler<Graphic>()
            .AddColorProperty("Color", (d, p) => d.color = p)
            .AddResourceProperty<Material>("Material", (d, p) => d.material = p);
        
        public static readonly IAttributeHandler<Graphic> Shadow = new AttributeHandler<EffectBatch>()
            .AddEnumProperty<ShadowType>("EffectType", (c, v) => c.type = v)
            .AddColorProperty("EffectColor", (c, v) => c.color = v)
            .AddVectorProperty("EffectOffset", (c, v) => c.offset = v)
            .AddBoolProperty("EffectUseGraphicAlpha", (c, v) => c.useGraphicAlpha = v)
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
            .AddResourceProperty<Sprite>("Sprite", (c, v) => c.sprite = v)
            .AddEnumProperty<Image.Type>("SpriteType", (c, v) => c.type = v);

        public static readonly IAttributeHandler<Text> Text = new AttributeHandler<Text>()
            .AddStringProperty("Text", (c, v) => c.text = v)
            .AddResourceProperty<Font>("Font", (c, v) => c.font = v)
            .AddEnumProperty<FontStyle>("FontStyle", (c, v) => c.fontStyle = v)
            .AddIntProperty("FontSize", (c, v) => c.fontSize = v)
            .AddIntProperty("LineSpacing", (c, v) => c.lineSpacing = v)
            .AddEnumProperty<TextAnchor>("Alignment", (c, v) => c.alignment = v)
            .AddBoolProperty("AlignByGeometry", (c, v) => c.alignByGeometry = v)
            .AddEnumProperty<HorizontalWrapMode>("HorizontalOverflow", (c, v) => c.horizontalOverflow = v)
            .AddEnumProperty<VerticalWrapMode>("VerticalOverflow", (c, v) => c.verticalOverflow = v)
            .AddBoolProperty("BestFit", (c, v) => c.resizeTextForBestFit = v)
            .AddIntProperty("MinSize", (c, v) => c.resizeTextMinSize = v)
            .AddIntProperty("MaxSize", (c, v) => c.resizeTextMaxSize = v);

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