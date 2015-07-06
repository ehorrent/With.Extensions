using System;

namespace With.CSharp.Tests.ClassPatterns
{
    public class Immutable_OtherNamingconvention
    {
        public readonly string m_FirstField;
        public readonly double m_SecondField;
        public readonly int m_ThirdField;

        public Immutable_OtherNamingconvention(string firstField, double secondField, int thirdField)
        {
            if (null == firstField) throw new ArgumentNullException("firstField");

            this.m_FirstField = firstField;
            this.m_SecondField = secondField;
            this.m_ThirdField = thirdField;
        }
    }
}
