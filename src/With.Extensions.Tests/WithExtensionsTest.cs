using System;
using NUnit.Framework;

namespace With.Extensions.Tests
{
    [TestFixture]
    public class WithExtensionTest
    {
        [Test]
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
        public void With_ClassWithMultipleCtors_Exception()
        {
            // Test
            var obj = new Immutable_MultipleCtors();
            obj.With(current => current.FirstField, "New first Value").Create();
        }

        [Test]
        [ExpectedException(exceptionType: typeof(ArgumentException))]
        public void With_LambdaSelectorReturnsConstant_Exception()
        {
            // Test
            var obj = Tuple.Create("First Value", 10);
            obj.With(_ => "Constant value", "New first Value").Create();
        }

        [Test]
        [ExpectedException(exceptionType: typeof(ArgumentException))]
        public void With_LambdaSelectorReturnsOtherInstanceMemberAccess_Exception()
        {
            // Test
            var obj = Tuple.Create("First Value", 10);
            var otherObj = Tuple.Create("Other first Value", 100);

            obj.With(_ => otherObj.Item1, "New first Value").Create();
        }

        [Test]
        public void With_ClassWithSingleConstructorAndMultipleStaticCtors_Ok()
        {
            const string newFirstValue = "New first Value";

            // Setup
            WithExtensions.ConstructorProvider = ctorInfos => args => new Immutable_StaticConstructors((string)args[0], (int)args[1]);

            // Test
            var obj = new Immutable_StaticConstructors("first value", 10);
            var obj2 = obj.With(current => current.FirstField, newFirstValue).Create();

            Assert.IsTrue(
                obj2.FirstField == newFirstValue &&
                obj2.SecondField == obj.SecondField);
        }

        [Test]
        public void With_OtherNamingConvention_Ok()
        {
            const string newFirstValue = "New first Value";

            // Setup
            WithExtensions.ConstructorProvider = ctorInfos => args => new Immutable_OtherNamingConvention((string)args[0], (double)args[1], (int)args[2]);

            // Test
            var obj = new Immutable_OtherNamingConvention("First Value", 2D, 3);
            var obj2 = obj.With(current => current.m_FirstField, newFirstValue).Create(name => string.Concat("m_", Naming.PascalCase.Convert(name)));

            Assert.IsTrue(
                obj2.m_FirstField == newFirstValue &&
                obj2.m_SecondField == obj.m_SecondField &&
                obj2.m_ThirdField == obj.m_ThirdField);
        }

        [Test]
        public void With_ChangeMutableObjectProperty_Ok()
        {
            const string newFirstValue = "New first Value";
            const string secondValue = "Second value";

            // Setup
            WithExtensions.ConstructorProvider = ctorInfos => args => new Mutable((string)args[0], (string)args[1]);

            // Test
            var obj = new Mutable("First Value", secondValue);
            var obj2 = obj.With(current => current.FirstField, newFirstValue).Create();

            Assert.IsTrue(
                obj2.FirstField == newFirstValue &&
                obj2.SecondField == obj.SecondField);
        }

        [Test]
        public void With_ChangeImmutableObjectField_Ok()
        {
            const string firstValue = "First Value";
            DateTime newSecondValue = new DateTime(2010, 2, 12);
            const int thirdValue = 120;

            // Setup
            WithExtensions.ConstructorProvider = ctorInfos => args => new Immutable((string)args[0], (DateTime)args[1], (int)args[2]);

            // Test
            var obj = new Immutable(firstValue, new DateTime(2000, 1, 1), thirdValue);
            var obj2 = obj.With(o => o.SecondField, newSecondValue).Create();

            Assert.IsTrue(
                obj2.FirstField == obj.FirstField &&
                obj2.SecondField == newSecondValue &&
                obj2.ThirdField == obj.ThirdField);
        }

        [Test]
        public void With_ChangeImmutableObjectProperty_Ok()
        {
            const string newFirstValue = "New first Value";
            const int secondValue = 10;

            // Setup
            WithExtensions.ConstructorProvider = ctorInfos => args => Tuple.Create((string)args[0], (int)args[1]);
            
            // Test
            var obj = Tuple.Create("First Value", secondValue);
            var obj2 = obj.With(o => o.Item1, newFirstValue).Create();

            Assert.IsTrue(
                obj2.Item1 == newFirstValue && 
                obj2.Item2 == obj.Item2);
        }

        [Test]
        public void With_ChainingCalls_Ok()
        {
            const string newFirstValue = "New first Value";
            const int secondValue = 10;
            const string newThirdValue = "New third Value";

            // Setup
            WithExtensions.ConstructorProvider = ctorInfos => args => Tuple.Create((string)args[0], (int)args[1], (string)args[2]);

            // Test
            var obj = Tuple.Create("First Value", secondValue, "Third value");
            var obj2 = obj.With(o => o.Item1, newFirstValue)
                          .With(o => o.Item3, newThirdValue)
                          .Create();

            Assert.IsTrue(
                obj2.Item1 == newFirstValue && 
                obj2.Item2 == obj.Item2 && 
                obj2.Item3 == newThirdValue);
        }

        [Test]
        public void With_Chaining_SingleCreation()
        {
            const string newFirstValue = "New first Value";
            const int secondValue = 10;
            const string newThirdValue = "New third Value";

            // Setup
            int callCount = 0;
            var mockInstanceProvider = (Constructor)(args =>
            {
                callCount++;
                return null;
            });

            WithExtensions.ConstructorProvider = ctorInfos => mockInstanceProvider;

            // Test
            var obj = Tuple.Create("First Value", secondValue, "Third value");
            var obj2 = obj.With(o => o.Item1, newFirstValue)
                          .With(o => o.Item3, newThirdValue)
                          .Create();

            Assert.AreEqual(1, callCount);
        }
    }
}