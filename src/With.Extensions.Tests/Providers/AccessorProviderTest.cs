using System;
using System.Collections.Generic;
using NUnit.Framework;
using With.Providers;

namespace With.Extensions.Tests.Providers
{
    [TestFixture]
    public class AccessorProviderTest
    {
        public static IEnumerable<Func<Type, string, PropertyOrFieldAccessor>> TestCases()
        {
            yield return ExpressionProviders.BuildPropertyOrFieldAccessor;
            yield return ReflectionProviders.GetPropertyOrFieldAccessor;
        }

        [TestCaseSource("TestCases")]
        [ExpectedException(exceptionType: typeof(ArgumentNullException))]
        public void BuildPropertyOrFieldProvider_NullArgument1_Exception(Func<Type, string, PropertyOrFieldAccessor> provider)
        {
            provider(null, "test");
        }

        [TestCaseSource("TestCases")]
        [ExpectedException(exceptionType: typeof(ArgumentNullException))]
        public void BuildPropertyOrFieldProvider_NullArgument2_Exception(Func<Type, string, PropertyOrFieldAccessor> provider)
        {
            provider(typeof(string), null);
        }
    }
}
