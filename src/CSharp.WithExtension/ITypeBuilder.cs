using System.Collections.Generic;

namespace System
{
    public interface ITypeBuilder
    {
        T Create<T>(IEnumerable<object> parameters);
    }
}