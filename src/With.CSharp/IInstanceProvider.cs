namespace With.CSharp
{
    /// <summary>
    /// Defines methods required to create class instances
    /// </summary>
    public interface IInstanceProvider
    {
        /// <summary>
        /// Create a new instance, using given constructor's arguments
        /// </summary>
        /// <typeparam name="T">Type of the instance to create</typeparam>
        /// <param name="ctorArguments">Ordered constructor's arguments</param>
        /// <returns>New instance</returns>
        T Create<T>(object[] ctorArguments) where T : class;
    }
}