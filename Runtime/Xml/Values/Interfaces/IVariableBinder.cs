using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IVariableBinder<in T> : IValueHandler
    {
        IBoundVariable Bind(XmlLayoutElement element, T instance, IVariableProvider provider);
    }
}