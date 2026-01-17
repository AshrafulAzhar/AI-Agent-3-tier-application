using MongoDB.Driver;
using System.Threading.Tasks;
using UserManagement.Domain.Interfaces;
using UserManagement.Domain.Models;

namespace UserManagement.Repository
{
    public class UserRepository : MongoRepository<User>, IUserRepository
    {
        public UserRepository(IMongoDatabase database) : base(database, "Users")
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.Email, email),
                Builders<User>.Filter.Eq(u => u.IsDeleted, false)
            );
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> GetByPhoneNumberAsync(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return null;

            var filter = Builders<User>.Filter.And(
                Builders<User>.Filter.Eq(u => u.PhoneNumber, phoneNumber),
                Builders<User>.Filter.Eq(u => u.IsDeleted, false)
            );
            return await Collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> GetDeletedUserByEmailOrPhoneAsync(string email, string phoneNumber)
        {
            var emailFilter = Builders<User>.Filter.Eq(u => u.Email, email);
            var phoneFilter = !string.IsNullOrEmpty(phoneNumber) 
                ? Builders<User>.Filter.Eq(u => u.PhoneNumber, phoneNumber) 
                : Builders<User>.Filter.Empty;

            var compositeFilter = Builders<User>.Filter.And(
                Builders<User>.Filter.Or(emailFilter, phoneFilter),
                Builders<User>.Filter.Eq(u => u.IsDeleted, true)
            );

            return await Collection.Find(compositeFilter).FirstOrDefaultAsync();
        }
    }
}
