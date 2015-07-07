using System;
using System.Reflection;

namespace With.CSharp.Factory
{
    public class DefaultConstructorProvider : IConstructorProvider
    {
        public Func<object[], T> GetProvider<T>(Type[] constructorSignature) where T : class
        {
            // Find constructor with matching argument types
            var ctor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                CallingConventions.HasThis,
                constructorSignature,
                new ParameterModifier[0]);

            // Create lambda
            Func<object[], T> creator = args => (T)ctor.Invoke(args);

            return creator;
        }
    }
}