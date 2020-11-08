using System.Collections.Generic;

namespace ThreeDISevenZeroR.XmlUI
{
    public class XmlComponentInfo : BaseXmlFactoryBuilder<XmlComponentInfo, XmlComponentInfo.XmlBuildFactory>, IXmlComponentInfo
    {
        public XmlComponentInfo(string name) : base(name) { }

        protected override XmlBuildFactory CreateEmptyFactory() => new XmlBuildFactory();
        public IXmlComponentFactory CreateFactory(Dictionary<string, string> attrs) => base.BuildFactory(attrs);
        
        public class XmlBuildFactory : ElementBuildFactory { }
    }
}