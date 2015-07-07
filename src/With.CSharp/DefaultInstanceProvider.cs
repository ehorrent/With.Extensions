using System;
using System.Linq;
using With.CSharp.Factory;

namespace With.CSharp
{
    public class DefaultInstanceProvider : IInstanceProvider
    {
        private readonly IConstructorProvider _factory;

        public DefaultInstanceProvider(IConstructorProvider factory)
        {
            if (null == factory) throw new ArgumentNullException("factory");

            _factory = factory;
        }

        public T Create<T>(object[] arguments) where T : class
        {
            var constructor = this._factory.GetProvider<T>(arguments.Select(arg => arg.GetType()).ToArray());
            return constructor(arguments);
        }
    }
}
