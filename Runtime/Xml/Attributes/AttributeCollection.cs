namespace ThreeDISevenZeroR.XmlUI
{
    public class AttributeCollection<T> : IAttributeCollection<T>
    {
        public IConstantSetter<T>[] SerializableConstants { get; set; }
        public IConstantSetter<T>[] NonSerializableConstants { get; set; }
        public IVariableBinder<T>[] Variables { get; set; }
    }
}