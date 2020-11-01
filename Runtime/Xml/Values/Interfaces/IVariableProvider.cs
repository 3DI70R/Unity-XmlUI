namespace ThreeDISevenZeroR.XmlUI
{
    public interface IVariableProvider
    {
        IVariableValue<T> GetValue<T>(string key);
    }
}