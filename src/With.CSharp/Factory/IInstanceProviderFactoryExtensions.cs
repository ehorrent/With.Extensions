using System;
using System.Linq;

namespace With.CSharp.Factory
{
    public static class IInstanceProviderExtensions
    {
        public static T Create<T>(this IInstanceProviderFactory instanceProvider, object[] arguments) where T : class
        {
            Func<object[], T> create = instanceProvider.GetProvider<T>(arguments.Select(arg => arg.GetType()).ToArray());
            return create(arguments);
        }
    }
}
