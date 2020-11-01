using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IAttributeInfo
    {
        string Name { get; }
        Type Type { get; }
        Type TargetType { get; }
        
        string FormatHint { get; }
    }
}