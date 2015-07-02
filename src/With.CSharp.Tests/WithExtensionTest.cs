using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace CSharp.WithExtension.Tests
{
    [TestFixture]
    public class WithExtensionTest
    {
        private class MultipleCtor
        {
            public readonly string FirstField;
            public readonly string SecondField;

            public MultipleCtor()
            {
            }

            public MultipleCtor(string firstField, string secondField)
            {
                this.FirstField = firstField;
                this.SecondField = secondField;
            }
        }

        private class ImmutableClass
        {
            public readonly string FirstField;
            public readonly int SecondField;

            public ImmutableClass(string firstField, int secondField)
            {
                this.FirstField = firstField;
                this.SecondField = secondField;
            }

            public ImmutableClass WithFirstField(string firstField)
            {
                return new ImmutableClass(firstField, this.SecondField);
            }
        }

        [Test]
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
        public void With_MultipleCtor_Exception()
        {
            // Setup
            var stubFactory = MockRepository.GenerateStub<IInstanceProviderFactory>();
            stubFactory.Stub(x => x.GetProvider<MultipleCtor>(new[] { typeof(string), typeof(string) }))
                       .Return(args => new MultipleCtor((string)args[0], (string)args[1]));

            System.WithExtension.Factory = stubFactory;

            // Test
            var obj = new MultipleCtor();
            obj.With(current => current.FirstField, "New first Value");
        }

        [Test]
        public void With_ChangeValue_Ok()
        {
            // Setup
            var stubFactory = MockRepository.GenerateStub<IInstanceProviderFactory>();
            stubFactory.Stub(x => x.GetProvider<ImmutableClass>(new[] { typeof(string), typeof(int) }))
                       .Return(args => new ImmutableClass((string)args[0], (int)args[1]));

            System.WithExtension.Factory = stubFactory;

            // Test
            var obj = new ImmutableClass("First Value", 10);
            var obj2 = obj.With(o => o.FirstField, "New first Value");

            Assert.IsTrue(obj2.FirstField == "New first Value" && obj2.SecondField == obj.SecondField);
        }
    }
}
