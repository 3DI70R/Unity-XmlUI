namespace ThreeDISevenZeroR.XmlUI
{
    public interface IConstantSetter<in T> : IValueHandler
    {
        void Apply(T instance);
    }
}