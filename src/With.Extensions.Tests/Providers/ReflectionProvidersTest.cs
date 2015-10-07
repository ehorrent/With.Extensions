using NUnit.Framework;
using With.Providers;

namespace With.Extensions.Tests
{
    [TestFixture]
    public class ReflectionProvidersTest
    {
        public class GetConstructorTest : AbstractConstructorProviderTest
        {
            public GetConstructorTest() : base(ReflectionProviders.GetConstructor) { }
        }

        public class GetPropertyOrFieldAccessorTest : AbstractAccessorProviderTest
        {
            public GetPropertyOrFieldAccessorTest() : base(ReflectionProviders.GetPropertyOrFieldAccessor) { }
        }
    }
}
