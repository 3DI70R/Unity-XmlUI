using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IConstantSetter<in T> : IValueHandler
    {
        bool IsSerializable { get; }
        Action<XmlLayoutElement, T> Setter { get; }
    }
}