namespace ThreeDISevenZeroR.XmlUI
{
    public interface IXmlComponentFactory
    {
        void BindAttrs(XmlLayoutElement element, 
            BoundAttributeCollection collection);
    }
}