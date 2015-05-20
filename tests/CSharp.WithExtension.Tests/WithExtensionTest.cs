using System;
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

        [Test]
        [ExpectedException(exceptionType: typeof(InvalidOperationException))]
        public void With_MultipleCtor_Exception()
        {
            var obj = new MultipleCtor();
            obj.With(current => current.A, "NEW A VALUE");
        }
    }
}
