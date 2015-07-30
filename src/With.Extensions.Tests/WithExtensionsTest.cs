using System;
using NUnit.Framework;
using Rhino.Mocks;
using With.Tests.ClassPatterns;

namespace With.Tests
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
            obj.With(current => current.FirstField, "New first Value").Create();
        }

        [Test]
        [ExpectedException(exceptionType: typeof(ArgumentException))]
        public void With_LambdaIsConstantAccess_Exception()
        {
            // Test
            var obj = Tuple.Create("First Value", 10);
            obj.With(_ => "Constant value", "New first Value").Create();
        }

        [Test]
        [ExpectedException(exceptionType: typeof(ArgumentException))]
        public void With_LambdaIsOtherInstanceMemberAccess_Exception()
        {
            // Test
            var obj = Tuple.Create("First Value", 10);
            var otherObj = Tuple.Create("Other first Value", 100);

            obj.With(_ => otherObj.Item1, "New first Value").Create();
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

            WithExtensions.InstanceProvider = stubProvider;

            // Test
            var obj = new Mutable("First Value", secondValue);
            var obj2 = obj.With(current => current.FirstField, newFirstValue).Create();

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

            WithExtensions.InstanceProvider = stubProvider;

            // Test
            var obj = new Immutable(firstValue, new DateTime(2000, 1, 1), thirdValue);
            var obj2 = obj.With(o => o.SecondField, newSecondValue).Create();

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
            
            WithExtensions.InstanceProvider = stubProvider;

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
            var stubProvider = MockRepository.GenerateStub<IInstanceProvider>();
            stubProvider.Stub(x => x.Create<Tuple<string, int, string>>(new object[] { newFirstValue, secondValue, newThirdValue }))
                        .Return(Tuple.Create(newFirstValue, secondValue, newThirdValue));

            WithExtensions.InstanceProvider = stubProvider;

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
            var mockProvider = MockRepository.GenerateMock<IInstanceProvider>();
            var createArg = new object[] {newFirstValue, secondValue, newThirdValue};
            mockProvider.Stub(x => x.Create<Tuple<string, int, string>>(createArg))
                        .Return(Tuple.Create(newFirstValue, secondValue, newThirdValue));

            WithExtensions.InstanceProvider = mockProvider;

            // Test
            var obj = Tuple.Create("First Value", secondValue, "Third value");
            var obj2 = obj.With(o => o.Item1, newFirstValue)
                          .With(o => o.Item3, newThirdValue)
                          .Create();

            mockProvider.AssertWasCalled(
                provider => provider.Create<Tuple<string, int, string>>(createArg),
                options => options.Repeat.Times(1)
            );
        }
    }
}