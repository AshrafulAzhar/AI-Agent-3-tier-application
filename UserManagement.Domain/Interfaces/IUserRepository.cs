using UserManagement.Domain.Models;
using System.Threading.Tasks;

namespace UserManagement.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByPhoneNumberAsync(string phoneNumber);
        Task<User> GetDeletedUserByEmailOrPhoneAsync(string email, string phoneNumber);
    }
}
