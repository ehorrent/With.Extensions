using System;
using System.Linq;
using With.CSharp.ConstructorProvider;

namespace With.CSharp
{
    /// <summary>
    /// Adapter on an IConstructorProvider to match IInstanceProvider definition
    /// </summary>
    public class InstanceProviderAdapter : IInstanceProvider
    {
        /// <summary>
        /// Constructor provider to adapt
        /// </summary>
        private readonly IConstructorProvider _constructorProvider;

        /// <summary>
        /// Create an instance of <see cref="T:InstanceProviderAdapter"/> type
        /// </summary>
        /// <param name="constructorProvider">Constructor provider to adapt</param>
        public InstanceProviderAdapter(IConstructorProvider constructorProvider)
        {
            if (null == constructorProvider) throw new ArgumentNullException("constructorProvider");

            this._constructorProvider = constructorProvider;
        }

        /// <summary>
        /// Create a new instance, using given constructor's arguments
        /// </summary>
        /// <typeparam name="T">Type of the instance to create</typeparam>
        /// <param name="ctorArguments">Ordered constructor's arguments</param>
        /// <returns>New instance</returns>
        public T Create<T>(object[] ctorArguments) where T : class
        {
            var constructor = this._constructorProvider.GetConstructor<T>(ctorArguments.Select(arg => arg.GetType()).ToArray());
            return constructor(ctorArguments);
        }
    }
}
