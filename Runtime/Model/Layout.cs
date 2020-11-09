namespace ThreeDISevenZeroR.XmlUI
{
    public struct Layout<T>
    {
        public readonly LayoutElement element;
        public readonly T component;

        public Layout(LayoutElement element, T component)
        {
            this.element = element;
            this.component = component;
        }

        public static implicit operator LayoutElement(Layout<T> l) => l.element;
        public static implicit operator T(Layout<T> l) => l.component;
    }
}