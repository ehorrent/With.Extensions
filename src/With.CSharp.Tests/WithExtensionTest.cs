using System;
using NUnit.Framework;
using Rhino.Mocks;
using With.CSharp.Factory;
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
            // Setup
            var stubFactory = MockRepository.GenerateStub<IInstanceProviderFactory>();
            stubFactory.Stub(x => x.GetProvider<Tuple<string, int>>(new[] { typeof(string), typeof(int) }))
                       .Return(args => new Tuple<string, int>((string)args[0], (int)args[1]));

            WithExtension.InstanceProvider = new DefaultInstanceProvider(stubFactory);

            // Test
            var obj = Tuple.Create("First Value", 10);
            var otherObj = Tuple.Create("Other first Value", 100);

            obj.With(_ => otherObj.Item1, "New first Value");
        }

        [Test]
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
        public void With_OtherNamingconvention_Exception()
        {
            // Setup
            var stubFactory = MockRepository.GenerateStub<IInstanceProviderFactory>();
            stubFactory.Stub(x => x.GetProvider<Immutable_OtherNamingConvention>(new[] { typeof(string), typeof(double), typeof(int) }))
                       .Return(args => new Immutable_OtherNamingConvention((string)args[0], (double)args[1], (int)args[2]));

            WithExtension.InstanceProvider = new DefaultInstanceProvider(stubFactory);
            
            // Test
            var obj = new Immutable_OtherNamingConvention("First Value", 2D, 3);
            obj.With(current => current.m_FirstField, "New first Value");
        }

        [Test]
        public void With_MutableChangeProperty_Ok()
        {
            // Setup
            var stubFactory = MockRepository.GenerateStub<IInstanceProviderFactory>();
            stubFactory.Stub(x => x.GetProvider<Mutable>(new[] { typeof(string), typeof(string) }))
                       .Return(args => new Mutable((string)args[0], (string)args[1]));

            WithExtension.InstanceProvider = new DefaultInstanceProvider(stubFactory);

            // Test
            var obj = new Mutable("First Value", "Second value");
            var obj2 = obj.With(current => current.FirstField, "New first Value");

            Assert.IsTrue(
                obj2.FirstField == "New first Value" &&
                obj2.SecondField == obj.SecondField);
        }

        [Test]
        public void With_ImmutableChangeField_Ok()
        {
            // Setup
            var stubFactory = MockRepository.GenerateStub<IInstanceProviderFactory>();
            stubFactory.Stub(x => x.GetProvider<Immutable>(new[] { typeof(string), typeof(DateTime), typeof(int) }))
                       .Return(args => new Immutable((string)args[0], (DateTime)args[1], (int)args[2]));

            WithExtension.InstanceProvider = new DefaultInstanceProvider(stubFactory);

            // Test
            var obj = new Immutable("First Value", new DateTime(2000, 01, 01), 120);
            var obj2 = obj.With(o => o.FirstField, "New first Value");

            Assert.IsTrue(
                obj2.FirstField == "New first Value" && 
                obj2.SecondField == obj.SecondField &&
                obj2.ThirdField == obj.ThirdField);
        }

        [Test]
        public void With_ImmutableChangeProperty_Ok()
        {
            // Setup
            var stubProvider = MockRepository.GenerateStub<IInstanceProvider>();
            stubProvider.Stub(x => x.Create<Tuple<string, int>>(new object[] { "New first Value", 10}))
                        .Return(Tuple.Create("New first Value", 10));
            
            WithExtension.InstanceProvider = stubProvider;

            // Test
            var obj = Tuple.Create("First Value", 10);
            var obj2 = obj.With(o => o.Item1, "New first Value");

            Assert.IsTrue(obj2.Item1 == "New first Value" && obj2.Item2 == obj.Item2);
        }
    }
}
