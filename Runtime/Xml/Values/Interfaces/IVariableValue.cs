using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IVariableValue<T>
    {
        T Value { get; }

        event Action<T> OnValueChanged;
    }
}