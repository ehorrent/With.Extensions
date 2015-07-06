namespace With.CSharp
{
    public interface IInstanceProvider
    {
        T Create<T>(object[] arguments) where T : class;
    }
}