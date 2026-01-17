using System;
using System.Threading.Tasks;
using UserManagement.Domain.DTOs;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Service.Validators
{
    public class IdentityValidator : IValidator<UserRegistrationRequest>
    {
        private readonly IUserRepository _userRepository;

        public IdentityValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task ValidateAsync(UserRegistrationRequest context)
        {
            var existingEmail = await _userRepository.GetByEmailAsync(context.Email);
            if (existingEmail != null) throw new Exception("Email is already in use.");

            if (!string.IsNullOrEmpty(context.PhoneNumber))
            {
                var existingPhone = await _userRepository.GetByPhoneNumberAsync(context.PhoneNumber);
                if (existingPhone != null) throw new Exception("Phone number is already in use.");
            }

            var deletedUser = await _userRepository.GetDeletedUserByEmailOrPhoneAsync(context.Email, context.PhoneNumber);
            if (deletedUser != null)
            {
                throw new Exception("This account was previously deleted. Please contact an admin for restoration.");
            }
        }
    }
}
