using System;

namespace With.Extensions.Tests
{
    public class Immutable_SubClass : Immutable
    {
        public readonly float FourthField;

        public Immutable_SubClass(string firstField, DateTime secondField, int thirdField, float fourthField)
            : base(firstField, secondField, thirdField)
        {
            this.FourthField = fourthField;
        }
    }
}
