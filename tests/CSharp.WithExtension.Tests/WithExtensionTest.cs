using System;
using CSharp.WithExtension.TypeBuilders;
using NUnit.Framework;

namespace CSharp.WithExtension.Tests
{
    [TestFixture]
    public class WithExtensionTest
    {
        private class MultipleCtor
        {
            public readonly string A;
            public readonly string B;

            public MultipleCtor()
            {
            }

            public MultipleCtor(string a, string b)
            {
                this.A = a;
                this.B = b;
            }
        }

        private class ImmutableClass
        {
            public readonly string A;
            public readonly int B;

            public ImmutableClass(string a, int b)
            {
                this.A = a;
                this.B = b;
            }            
        }

        [Test]
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
        public void With_MultipleCtor_Exception()
        {

            var obj = new MultipleCtor();
            obj.With(current => current.A, "NEW A VALUE");
        }

        [Test]
        public void With_ChangeValue_Ok()
        {
            System.WithExtension.Register(new SlowTypeBuilder());

            var obj = new ImmutableClass("TEMP", 2);
            var obj2 = obj.With(o => o.A, "TEST");

            Assert.IsTrue(obj2.A == "TEST" && obj2.B == obj.B);
        }
    }
}
