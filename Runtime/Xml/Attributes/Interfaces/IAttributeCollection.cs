namespace ThreeDISevenZeroR.XmlUI
{
    public interface IAttributeCollection<in T>
    {
        IConstantSetter<T>[] Constants { get; }
        IVariableBinder<T>[] Variables { get; }
    }
}