using System;
using System.Diagnostics;
using System.Reflection;
using With.Providers;

namespace With.Extensions.Performances
{
    class Program
    {
        const int TestCount = 100000;
        const string newFirstValue = "New first Value";
        const int secondValue = 10;
        const string newThirdValue = "New third Value";

        static void Main(string[] args)
        {
            // Test
            var obj = Tuple.Create("First Value", secondValue, "Third value");

            // Create query
            var query = obj.With(o => o.Item1, newFirstValue)
                           .With(o => o.Item3, newThirdValue);

            // Test creation
            Action action = () => query.Create();
            RecordResults(action);
            //Diagnose(action);
        }

        private static void Diagnose(Action action)
        {
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();
            for (int i = 0; i < TestCount; ++i)
                action();
        }

        private static void RecordResults(Action action)
        {
            WithExtensions.ConstructorProvider = Helpers.Cache.Memoize<ConstructorInfo, Constructor>(ExpressionProviders.BuildConstructor);
            WithExtensions.AccessorProvider = Helpers.Cache.Memoize<Type, string, PropertyOrFieldAccessor>(ExpressionProviders.BuildPropertyOrFieldAccessor);
            action();

            var stopWatch1 = new Stopwatch();
            stopWatch1.Start();
            for (int i = 0; i < TestCount; ++i)
                action();
            stopWatch1.Stop();
            Console.WriteLine("Expression + cache : " + stopWatch1.Elapsed);

            WithExtensions.ConstructorProvider = ReflectionProviders.GetConstructor;
            WithExtensions.AccessorProvider = ReflectionProviders.GetPropertyOrFieldAccessor;
            var stopWatch2 = new Stopwatch();
            stopWatch2.Start();
            for (int i = 0; i < TestCount; ++i)
                action();
            stopWatch2.Stop();
            Console.WriteLine("Réflection pure : " + stopWatch2.Elapsed);

            Action action3 = () => Tuple.Create(newFirstValue, secondValue, newThirdValue);
            var stopWatch3 = new Stopwatch();
            stopWatch3.Start();
            for (int i = 0; i < TestCount; ++i)
                action3();
            stopWatch3.Stop();
            Console.WriteLine("Appel direct : " + stopWatch3.Elapsed);
        }
    }
}
