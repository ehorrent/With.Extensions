namespace With
{
    /// <summary>
    /// Create a new instance, using specified constructor's arguments
    /// </summary>
    /// <param name="arguments">Ordered constructor's arguments</param>
    /// <returns>New instance</returns>
    public delegate object Constructor(object[] arguments);

    /// <summary>
    /// Returns a property/field value of the specified object
    /// </summary>
    /// <param name="obj">The object whose property/field value will be returned</param>
    /// <returns>The property/field value of the specified object</returns>
    public delegate object PropertyOrFieldProvider(object obj);
}
