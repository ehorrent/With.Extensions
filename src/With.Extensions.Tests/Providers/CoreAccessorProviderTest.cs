using System;
using System.Collections.Generic;
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

        [Test]
        [ExpectedException]
        public void AccessorProvider_PropertyOrFieldNameDoesntExist_Exception()
        {
            var accessor = this._provider(typeof(Tuple<string, string>), "Item3");
            accessor(Tuple.Create("Item1 value", "Item2 value"));
        }

        [Test]
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
        public void AccessorProvider_PropertyOrFieldNameWithInvalidCase_Exception()
        {
            var tuple = Tuple.Create("Item1 value", "Item2 value");
            var accessor = this._provider(typeof(Tuple<string, string>), "ITEM1");
            var value = accessor(tuple);
        }

        [Test]
        public void AccessorProvider_NonPublicField_Ok()
        {
            const string item2Value = "Item2 value";
            var tuple = Tuple.Create("Item1 value", item2Value);
            var accessor = this._provider(typeof(Tuple<string, string>), "m_Item2");
            var value = (string)accessor(tuple);

            Assert.IsTrue(value == item2Value);
        }

        [Test]
        public void AccessorProvider_PublicField_Ok()
        {
            var thirdFieldValue = 10;
            var immutable = new Immutable("first field value", DateTime.MaxValue, thirdFieldValue);
            var accessor = this._provider(typeof(Immutable), "ThirdField");
            var value = (int)accessor(immutable);

            Assert.IsTrue(value == thirdFieldValue);
        }

        [Test]
        public void AccessorProvider_NonPublicProperty_Ok()
        {
            var secondFieldValue = new List<float> { 1.0f, 2.0f, 3.0f };
            var immutable = new Immutable_NonPublicProperties("first field value", secondFieldValue);
            var accessor = this._provider(typeof(Immutable_NonPublicProperties), "SecondField");
            var value = (List<float>)accessor(immutable);

            Assert.IsTrue(value == secondFieldValue);
        }

        [Test]
        public void AccessorProvider_PublicProperty_Ok()
        {
            const string item2Value = "Item2 value";
            var tuple = Tuple.Create("Item1 value", item2Value);
            var accessor = this._provider(typeof(Tuple<string, string>), "Item2");
            var value = (string)accessor(tuple);

            Assert.IsTrue(value == item2Value);
        }
    }
}
