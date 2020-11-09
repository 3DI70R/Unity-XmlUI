using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IVariableBinder<in T> : IValueHandler
    {
        IBoundVariable Bind(LayoutElement element, T instance, IVariableProvider provider);
    }
}