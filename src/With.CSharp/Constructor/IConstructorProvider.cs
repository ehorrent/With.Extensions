using System;

namespace With.CSharp.ConstructorProvider
{
    /// <summary>
    /// Defines methods required to provide constructor Funcs on a given type
    /// </summary>
    public interface IConstructorProvider
    {
        /// <summary>
        /// Provides a constructor, based on the given signature
        /// </summary>
        /// <typeparam name="T">Type of the instance to be created by the constructor</typeparam>
        /// <param name="constructorSignature">Constructor's signature</param>
        /// <returns>Corresponding constructor (if existing)</returns>
        Func<object[], T> GetConstructor<T>(Type[] constructorSignature) where T : class;
    }
}