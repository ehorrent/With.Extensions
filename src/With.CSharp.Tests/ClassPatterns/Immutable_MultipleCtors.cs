namespace With.CSharp.Tests.ClassPatterns
{
    public class Immutable_MultipleCtors
    {
        public readonly string FirstField;
        public readonly string SecondField;

        public Immutable_MultipleCtors()
        {
            this.FirstField = string.Empty;
            this.SecondField = string.Empty;
        }

        public Immutable_MultipleCtors(string firstField, string secondField)
        {
            this.FirstField = firstField;
            this.SecondField = secondField;
        }
    }
}
