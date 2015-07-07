using System;
using NUnit.Framework;
using Rhino.Mocks;
using With.CSharp.Tests.ClassPatterns;

namespace With.CSharp.Tests
{
    [TestFixture]
    public class WithExtensionTest
    {
        [Test]
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
        public void With_MultipleCtors_Exception()
        {
            // Test
            var obj = new Immutable_MultipleCtors();
            obj.With(current => current.FirstField, "New first Value");
        }

        [Test]
        [ExpectedException(exceptionType: typeof(ArgumentException))]
        public void With_LambdaIsConstantAccess_Exception()
        {
            // Test
            var obj = Tuple.Create("First Value", 10);
            obj.With(_ => "Constant value", "New first Value");
        }

        [Test]
        [ExpectedException(exceptionType: typeof(ArgumentException))]
        public void With_LambdaIsOtherInstanceMemberAccess_Exception()
        {
            // Test
            var obj = Tuple.Create("First Value", 10);
            var otherObj = Tuple.Create("Other first Value", 100);

            obj.With(_ => otherObj.Item1, "New first Value");
        }

        [Test]
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
        public void With_OtherNamingConvention_Exception()
        {         
            // Test
            var obj = new Immutable_OtherNamingConvention("First Value", 2D, 3);
            obj.With(current => current.m_FirstField, "New first Value");
        }

        [Test]
        public void With_MutableChangeProperty_Ok()
        {
            const string newFirstValue = "New first Value";
            const string secondValue = "Second value";

            // Setup
            var stubProvider = MockRepository.GenerateStub<IInstanceProvider>();
            stubProvider.Stub(x => x.Create<Mutable>(new object[] { newFirstValue, secondValue }))
                        .Return(new Mutable(newFirstValue, secondValue));

            WithExtension.InstanceProvider = stubProvider;

            // Test
            var obj = new Mutable("First Value", secondValue);
            var obj2 = obj.With(current => current.FirstField, newFirstValue);

            Assert.IsTrue(
                obj2.FirstField == newFirstValue &&
                obj2.SecondField == obj.SecondField);
        }

        [Test]
        public void With_ImmutableChangeField_Ok()
        {
            const string firstValue = "First Value";
            DateTime newSecondValue = new DateTime(2010, 2, 12);
            const int thirdValue = 120;

            // Setup
            var stubProvider = MockRepository.GenerateStub<IInstanceProvider>();
            stubProvider.Stub(x => x.Create<Immutable>(new object[] { firstValue, newSecondValue, thirdValue }))
                        .Return(new Immutable(firstValue, newSecondValue, thirdValue));

            WithExtension.InstanceProvider = stubProvider;

            // Test
            var obj = new Immutable(firstValue, new DateTime(2000, 1, 1), thirdValue);
            var obj2 = obj.With(o => o.SecondField, newSecondValue);

            Assert.IsTrue(
                obj2.FirstField == obj.FirstField &&
                obj2.SecondField == newSecondValue &&
                obj2.ThirdField == obj.ThirdField);
        }

        [Test]
        public void With_ImmutableChangeProperty_Ok()
        {
            const string newFirstValue = "New first Value";
            const int secondValue = 10;

            // Setup
            var stubProvider = MockRepository.GenerateStub<IInstanceProvider>();
            stubProvider.Stub(x => x.Create<Tuple<string, int>>(new object[] {newFirstValue, secondValue}))
                        .Return(Tuple.Create(newFirstValue, secondValue));
            
            WithExtension.InstanceProvider = stubProvider;

            // Test
            var obj = Tuple.Create("First Value", secondValue);
            var obj2 = obj.With(o => o.Item1, newFirstValue);

            Assert.IsTrue(obj2.Item1 == newFirstValue && obj2.Item2 == obj.Item2);
        }
    }
}
