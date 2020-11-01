using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IVariableValue<out T>
    {
        T Value { get; }

        event Action<T> OnValueChanged;
    }
}