namespace System
{
    public interface ITypeBuilder
    {
        T Create<T>(object[] parameters);
    }
}