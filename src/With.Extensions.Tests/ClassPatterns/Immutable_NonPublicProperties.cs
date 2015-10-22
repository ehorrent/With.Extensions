using System;
using System.Collections.Generic;

namespace With.Extensions.Tests
{
    public class Immutable_NonPublicProperties
    {
        private readonly string _firstField;
        private readonly IEnumerable<float> _secondField;

        protected string FirstField
        {
            get { return this._firstField; }
        }

        protected IEnumerable<float> SecondField
        {
            get { return this._secondField; }
        }

        public Immutable_NonPublicProperties(string firstField, IEnumerable<float> secondField)
        {
            if (null == firstField) throw new ArgumentNullException("firstField");
            if (null == secondField) throw new ArgumentNullException("secondField");

            this._firstField = firstField;
            this._secondField = secondField;
        }
    }
}
