using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharp.WithExtension.TypeBuilders
{
    public class SlowTypeBuilder : ITypeBuilder
    {
        public T Create<T>(IEnumerable<object> arguments)
        {
            var ctors = typeof(T).GetConstructors();
            var ctor = ctors.First(c =>
            {
                var parameters = c.GetParameters();
                if (parameters.Length != arguments.Count())
                    return false;

                var args = arguments.ToArray();
                return parameters.Zip(args, Tuple.Create)
                                 .All(tuple => tuple.Item1.ParameterType == tuple.Item2.GetType());
            });

            return (T)ctor.Invoke(arguments.ToArray());
        }
    }
}