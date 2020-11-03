using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IConstantSetter<in T> : IValueHandler
    {
        Action<T> SetterDelegate { get; }
        
        void Apply(T instance);
    }
}