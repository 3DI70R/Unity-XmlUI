using UnityEngine;
using UnityEngine.UI;

namespace ThreeDISevenZeroR.XmlUI
{
    public static class AttributeHandlers
    {
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

        // Layout
        
        public static readonly IAttributeHandler<ContentSizeFitter> ContentSizeFitter = 
            new AttributeHandler<ContentSizeFitter>()
                .AddEnumProperty<ContentSizeFitter.FitMode>("HorizontalFit", (c, v) => c.horizontalFit = v)
                .AddEnumProperty<ContentSizeFitter.FitMode>("VerticalFit", (c, v) => c.verticalFit = v);

        public static readonly IAttributeHandler<LayoutElement> LayoutElement = new AttributeHandler<LayoutElement>()
            .AddFloatProperty("MinWidth", (c, v) => c.minWidth = v)
            .AddFloatProperty("MinHeight", (c, v) => c.minHeight = v)
            .AddFloatProperty("PreferredWidth", (c, v) => c.preferredWidth = v)
            .AddFloatProperty("PreferredHeight", (c, v) => c.preferredHeight = v)
            .AddFloatProperty("FlexibleWidth", (c, v) => c.flexibleWidth = v)
            .AddFloatProperty("FlexibleHeight", (c, v) => c.flexibleHeight = v);

        public static readonly IAttributeHandler<HorizontalOrVerticalLayoutGroup> HorizontalOrVerticalLayoutGroup =
            new AttributeHandler<HorizontalOrVerticalLayoutGroup>()
                .AddBoolProperty("ReverseArrangement", (c, v) => c.reverseArrangement = v)
                .AddBoolProperty("ChildControlWidth", (c, v) => c.childControlWidth = v)
                .AddBoolProperty("ChildControlHeight", (c, v) => c.childControlHeight = v)
                .AddBoolProperty("ChildScaleWidth", (c, v) => c.childScaleWidth = v)
                .AddBoolProperty("ChildScaleHeight", (c, v) => c.childScaleHeight = v)
                .AddBoolProperty("ChildForceExpandWidth", (c, v) => c.childForceExpandWidth = v)
                .AddBoolProperty("ChildForceExpandHeight", (c, v) => c.childForceExpandHeight = v)
                .AddIntProperty("Spacing", (c, v) => c.spacing = v);

        public static readonly IAttributeHandler<LayoutGroup> LayoutGroup = new AttributeHandler<GroupBatch>()
            .AddEnumProperty<TextAnchor>("ChildAlignment", (c, v) => c.childAlignment = v)
            .AddIntProperty("Padding", (c, v) => { c.left = v; c.right = v; c.top = v; c.bottom = v; })
            .AddIntProperty("PaddingHorizontal", (c, v) => { c.left = v; c.right = v; })
            .AddIntProperty("PaddingVertical", (c, v) => { c.top = v; c.bottom = v; })
            .AddIntProperty("PaddingLeft", (c, v) => c.left = v)
            .AddIntProperty("PaddingRight", (c, v) => c.right = v)
            .AddIntProperty("PaddingTop", (c, v) => c.top = v)
            .AddIntProperty("PaddingBottom", (c, v) => c.bottom = v)
            .AsBatchForObject<LayoutGroup>(new GroupBatch(),
                getter: (o, b) =>
                {
                    var p = o.padding;
                    b.left = p.left;
                    b.right = p.right;
                    b.top = p.top;
                    b.bottom = p.bottom;
                    b.childAlignment = o.childAlignment;
                },
                setter: (o, b) =>
                {
                    o.padding = new RectOffset(b.left, b.right, b.top, b.bottom);
                    o.childAlignment = b.childAlignment;
                });
        
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
        
        private class GroupBatch
        {
            public int left;
            public int right;
            public int top;
            public int bottom;
            public TextAnchor childAlignment;
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