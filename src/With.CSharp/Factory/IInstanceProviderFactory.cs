using System;

namespace With.CSharp.Factory
{
    public interface IInstanceProviderFactory
    {
        Func<object[], T> GetProvider<T>(Type[] constructorSignature) where T : class;
    }
}