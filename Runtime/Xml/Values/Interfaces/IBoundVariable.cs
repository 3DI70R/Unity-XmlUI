using System;

namespace ThreeDISevenZeroR.XmlUI
{
    public interface IBoundVariable
    {
        event Action OnUpdated;
        
        void Apply();
        void Unbind();
    }
}