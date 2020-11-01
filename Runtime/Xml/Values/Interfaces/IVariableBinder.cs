using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IVariableBinder<in T> : IValueHandler
    {
        IBoundVariable Bind(T instance, IVariableProvider provider);
    }
}