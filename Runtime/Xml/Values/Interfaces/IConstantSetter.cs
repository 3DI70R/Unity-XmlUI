using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IConstantSetter<in T> : IValueHandler
    {
        bool IsSerializable { get; }
        Action<T> SetterDelegate { get; }
        
        void Apply(T instance);
    }
}