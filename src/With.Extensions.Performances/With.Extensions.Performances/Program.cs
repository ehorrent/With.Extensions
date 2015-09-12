using System;
using System.Diagnostics;

namespace With.Extensions.Performances
{
    class Program
    {
        static void Main(string[] args)
        {
            const string newFirstValue = "New first Value";
            const int secondValue = 10;
            const string newThirdValue = "New third Value";

            // Test
            var obj = Tuple.Create("First Value", secondValue, "Third value");

            var obj2 = obj.With(o => o.Item1, newFirstValue)
                          .With(o => o.Item3, newThirdValue)
                          .Create();

            for (int i = 0; i < 100000; ++i)
            {
                obj2 = obj.With(o => o.Item1, newFirstValue)
                          .With(o => o.Item3, newThirdValue)
                          .Create();
            }
        }
    }
}
