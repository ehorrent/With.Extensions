using System.Threading.Tasks;

namespace System
{
    public interface ITypeBuilder
    {
        Task Create<T>(object[] parameters);
    }
}