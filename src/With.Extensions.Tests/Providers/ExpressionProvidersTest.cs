using NUnit.Framework;
using With.Providers;

namespace With.Extensions.Tests
{
    [TestFixture]
    public class ExpressionProvidersTest
    {
        public class BuildConstructorTest : AbstractConstructorProviderTest
        {
            public BuildConstructorTest() : base(ExpressionProviders.BuildConstructor) { }
        }

        public class BuildPropertyOrFieldAccessorTest : AbstractAccessorProviderTest
        {
            public BuildPropertyOrFieldAccessorTest() : base(ExpressionProviders.BuildPropertyOrFieldAccessor) { }
        }
    }
}
