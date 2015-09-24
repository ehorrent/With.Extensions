using System;
using NUnit.Framework;
using With.Providers;

namespace With.Extensions.Tests.Providers
{
    [TestFixture]
    public class ExpressionProvidersTest
    {
        public class BuildConstructor
        {
            [Test]
            [ExpectedException(exceptionType: typeof(ArgumentNullException))]
            public void BuildConstructor_NullArgument_Exception()
            {
                ExpressionProviders.BuildConstructor(null);
            }
        }

        public class BuildPropertyOrFieldProvider
        {
            [Test]
            [ExpectedException(exceptionType: typeof(ArgumentNullException))]
            public void BuildPropertyOrFieldProvider_NullArgument_Exception()
            {
                ExpressionProviders.BuildConstructor(null);
            }
        }
    }
}
