using System.Threading.Tasks;

namespace UserManagement.Domain.Interfaces
{
    public interface IValidator<T>
    {
        Task ValidateAsync(T context);
    }
}
