using System;

namespace With.CSharp.Factory
{
    public interface IConstructorProvider
    {
        Func<object[], T> GetProvider<T>(Type[] constructorSignature) where T : class;
    }
}