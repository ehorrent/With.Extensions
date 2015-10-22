using System;

namespace With.Extensions.Tests
{
    public class Immutable
    {
        public readonly string FirstField;
        public readonly DateTime SecondField;
        public readonly int ThirdField;

        public Immutable(string firstField, DateTime secondField, int thirdField)
        {
            if (null == firstField) throw new ArgumentNullException("firstField");

            this.FirstField = firstField;
            this.SecondField = secondField;
            this.ThirdField = thirdField;
        }
    }
}
