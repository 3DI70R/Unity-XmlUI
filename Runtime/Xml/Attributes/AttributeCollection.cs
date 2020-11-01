namespace ThreeDISevenZeroR.XmlUI
{
    public class AttributeCollection<T> : IAttributeCollection<T>
    {
        public IConstantSetter<T>[] Constants { get; set; }
        public IVariableBinder<T>[] Variables { get; set; }
    }
}