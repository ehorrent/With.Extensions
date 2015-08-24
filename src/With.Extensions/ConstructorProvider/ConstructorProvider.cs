using System.Reflection;

namespace With.ConstructorProvider
{
    /// <summary>
    /// Create a new instance, using given constructor's arguments
    /// </summary>
    /// <param name="arguments">Ordered constructor's arguments</param>
    /// <returns>New instance</returns>
    public delegate object Constructor(object[] arguments);

    /// <summary>
    /// Creates a new Constructor
    /// </summary>
    /// <param name="ctor">Informations used to create the constructor delegate</param>
    /// <returns>New constructor</returns>
    public delegate Constructor GetConstructor(ConstructorInfo ctor);
}