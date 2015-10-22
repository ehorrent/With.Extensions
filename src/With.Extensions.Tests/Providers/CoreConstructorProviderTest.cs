using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace With.Extensions.Tests
{
    public abstract class AbstractConstructorProviderTest
    {
        private readonly Func<ConstructorInfo, Constructor> _provider;

        protected AbstractConstructorProviderTest(Func<ConstructorInfo, Constructor> provider)
        {
            _provider = provider;
        }

        [Test]
        [ExpectedException(exceptionType: typeof(ArgumentNullException))]
        public void ConstructorProvider_ArgIsNull_Exception()
        {
            this._provider(null);
        }

        [Test]
        [ExpectedException]
        public void ConstructorProvider_TooManyArguments_Exception()
        {
            var typeInfo = typeof(Immutable).GetTypeInfo();
            var ctorInfo = typeInfo.DeclaredConstructors.First();
            var constructor = this._provider(ctorInfo);

            constructor(new object[] { string.Empty, DateTime.Now, 0, "Invalid argument" });
        }

        [Test]
        [ExpectedException]
        public void ConstructorProvider_NotEnoughArguments_Exception()
        {
            var typeInfo = typeof(Immutable).GetTypeInfo();
            var ctorInfo = typeInfo.DeclaredConstructors.First();
            var constructor = this._provider(ctorInfo);

            constructor(new object[] { string.Empty });
        }

        [Test]
        [ExpectedException]
        public void ConstructorProvider_InvalidArgumentType_Exception()
        {
            var typeInfo = typeof(Immutable).GetTypeInfo();
            var ctorInfo = typeInfo.DeclaredConstructors.First();
            var constructor = this._provider(ctorInfo);

            constructor(new object[] { string.Empty, "Invalid argument type", 0 });
        }

        [Test]
        public void ConstructorProvider_MatchingArgumentTypes_OK()
        {
            const string firstFieldValue = "First value";
            DateTime secondFieldValue = DateTime.MaxValue;
            const int thirdFieldValue = 10;

            var typeInfo = typeof(Immutable).GetTypeInfo();
            var ctorInfo = typeInfo.DeclaredConstructors.First();
            var constructor = this._provider(ctorInfo);

            var newObj = (Immutable)constructor(new object[] { firstFieldValue, secondFieldValue, thirdFieldValue });
            Assert.IsTrue(
                newObj.GetType() == typeof(Immutable) &&
                newObj.FirstField == firstFieldValue &&
                newObj.SecondField == secondFieldValue &&
                newObj.ThirdField == thirdFieldValue);
        }

        [Test]
        public void ConstructorProvider_MatchingSubClassArgumentTypes_OK()
        {
            var typeInfo = typeof(Tuple<string, Immutable>).GetTypeInfo();
            var ctorInfo = typeInfo.DeclaredConstructors.First();
            var constructor = this._provider(ctorInfo);

            var item1 = "Item1 value";
            var subClassItem2 = new Immutable_SubClass("Item2", new DateTime(2015, 1, 1), 1, 1.0f);
            var newObj = (Tuple<string, Immutable>)constructor(new object[] { item1, subClassItem2 });

            Assert.IsTrue(
                newObj.GetType() == typeof(Tuple<string, Immutable>) &&
                newObj.Item1 == item1 &&
                newObj.Item2 == subClassItem2);
        }
    }
}
