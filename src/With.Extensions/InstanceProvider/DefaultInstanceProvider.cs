using System.Linq;
using System.Reflection;

namespace With
{
    /// <summary>
    /// Default instance provider, using reflection to create new instances
    /// </summary>
    public class DefaultInstanceProvider : IInstanceProvider
    {
        /// <summary>
        /// Create a new instance, using given constructor's arguments
        /// </summary>
        /// <typeparam name="T">Type of the instance to create</typeparam>
        /// <param name="ctorArguments">Ordered constructor's arguments</param>
        /// <returns>New instance</returns>
        public T Create<T>(object[] ctorArguments) where T : class
        {
            // Find constructor with matching argument types
            var ctor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                CallingConventions.HasThis,
                ctorArguments.Select(arg => arg.GetType()).ToArray(),
                new ParameterModifier[0]);

            // Create instance
            return (T)ctor.Invoke(ctorArguments);
        }
    }
}
