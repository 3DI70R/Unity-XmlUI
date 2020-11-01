namespace ThreeDISevenZeroR.XmlUI
{
    public delegate void BatchGetter<O, B>(O instance, B batch);
    public delegate void BatchSetter<O, B>(O instance, B batch);
    
    public delegate void ValueSetterDelegate<in T, in P>(T instance, P value);
}