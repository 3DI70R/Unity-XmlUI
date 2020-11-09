using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IConstantSetter<in T> : IValueHandler
    {
        bool IsSerializable { get; }
        Action<LayoutElement, T> Setter { get; }
    }
}