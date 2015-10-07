using System;
using NUnit.Framework;

namespace With.Extensions.Tests
{
    public abstract class AbstractAccessorProviderTest
    {
        private readonly Func<Type, string, PropertyOrFieldAccessor> _provider;

        protected AbstractAccessorProviderTest(Func<Type, string, PropertyOrFieldAccessor> provider)
        {
            _provider = provider;
        }

        [Test]
        [ExpectedException(exceptionType: typeof(ArgumentNullException))]
        public void AccessorProvider_Arg1IsNull_Exception()
        {
            this._provider(null, "test");
        }

        [Test]
        [ExpectedException(exceptionType: typeof(ArgumentNullException))]
        public void AccessorProvider_Arg02IsNull_Exception()
        {
            this._provider(typeof(string), null);
        }
    }
}
