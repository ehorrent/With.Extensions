using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using With.Providers;

namespace With.Extensions.Tests.Providers
{
    [TestFixture]
    public class ConstructorProviderTest
    {
        public static IEnumerable<Func<ConstructorInfo, Constructor>> TestCases()
        {
            yield return ExpressionProviders.BuildConstructor;
            yield return ReflectionProviders.GetConstructor;
        }

        [TestCaseSource("TestCases")]
        [ExpectedException(exceptionType: typeof(ArgumentNullException))]
        public void GetConstructor_NullArgument_Exception(Func<ConstructorInfo, Constructor> provider)
        {
            provider(null);
        }
    }
}
