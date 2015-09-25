using System;
using System.Diagnostics;
using System.Reflection;
using With.Providers;

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

            var query = obj.With(o => o.Item1, newFirstValue)
                           .With(o => o.Item3, newThirdValue);

            Action action = () => query.Create();

            WithExtensions.ConstructorProvider = Helpers.Cache.Memoize<ConstructorInfo, Constructor>(ExpressionProviders.BuildConstructor);
            WithExtensions.PropertyOrFieldProvider = Helpers.Cache.Memoize<Type, string, PropertyOrFieldProvider>(ExpressionProviders.BuildPropertyOrFieldProvider);
            action();

            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey();

            for (int i = 0; i < 100; ++i)
                action();

            /*WithExtensions.ConstructorProvider = ReflectionProviders.GetConstructor;
            WithExtensions.PropertyOrFieldProvider = ReflectionProviders.GetPropertyOrFieldProvider;
            var stopWatch2 = new Stopwatch();
            stopWatch2.Start();
            for (int i = 0; i < 10000000; ++i)
                action();
            stopWatch2.Stop();
            Console.WriteLine("Réflection pure : " + stopWatch2.Elapsed);

            Action action3 = () => Tuple.Create(newFirstValue, secondValue, newThirdValue);
            var stopWatch3 = new Stopwatch();
            stopWatch3.Start();
            for (int i = 0; i < 10000000; ++i)
                action3();
            stopWatch3.Stop();
            Console.WriteLine("Appel direct : " + stopWatch3.Elapsed);
            */
            ////Console.ReadKey();
        }
    }
}
