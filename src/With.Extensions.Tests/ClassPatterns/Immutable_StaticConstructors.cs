using System;

namespace With.Extensions.Tests
{
    public class Immutable_StaticConstructors
    {
        public readonly string FirstField;
        public readonly int SecondField;

        static Immutable_StaticConstructors()
        {
        }

        public static Immutable_StaticConstructors Empty = new Immutable_StaticConstructors(string.Empty, int.MinValue);

        public Immutable_StaticConstructors(string firstField, int secondField)
        {
            if (null == firstField) throw new ArgumentNullException("firstField");

            this.FirstField = firstField;
            this.SecondField = secondField;
        }
    }
}
