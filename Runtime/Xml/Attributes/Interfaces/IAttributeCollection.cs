namespace ThreeDISevenZeroR.XmlUI
{
    public interface IAttributeCollection<in T>
    {
        IConstantSetter<T>[] SerializableConstants { get; }
        IConstantSetter<T>[] NonSerializableConstants { get; }
        IVariableBinder<T>[] Variables { get; }
    }
}