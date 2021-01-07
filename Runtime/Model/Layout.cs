namespace ThreeDISevenZeroR.XmlUI
{
    public struct Layout<T>
    {
        public readonly XmlLayoutElement element;
        public readonly T component;

        public Layout(XmlLayoutElement element, T component)
        {
            this.element = element;
            this.component = component;
        }

        public static implicit operator XmlLayoutElement(Layout<T> l) => l.element;
        public static implicit operator T(Layout<T> l) => l.component;
    }
}