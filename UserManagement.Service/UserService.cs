using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserManagement.Domain.DTOs;
using UserManagement.Domain.Interfaces;
using UserManagement.Domain.Models;
using BC = BCrypt.Net.BCrypt;

namespace UserManagement.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly string[] _weakPasswords = { "Password123!", "Admin123!", "Welcome123!" };

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponse> RegisterUserAsync(UserRegistrationRequest request)
        {
            // 1. Normalization
            var normalizedEmail = NormalizeEmail(request.Email);
            var normalizedPhone = NormalizePhone(request.PhoneNumber);

            // 2. Uniqueness & Soft-delete checks
            await ValidateUniquenessAsync(normalizedEmail, normalizedPhone);

            // 3. Password Policy Validation
            ValidatePasswordPolicy(request.Password, normalizedEmail, normalizedPhone);

            // 4. Age Policy (13+)
            ValidateAgePolicy(request.DateOfBirth);

            // 5. Build User Model
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) 
                    ? $"{request.FirstName} {request.LastName}" 
                    : request.DisplayName.Trim(),
                Email = normalizedEmail,
                PhoneNumber = normalizedPhone,
                PasswordHash = BC.HashPassword(request.Password),
                DateOfBirth = request.DateOfBirth,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow
            };

            // 6. Persist
            await _userRepository.AddAsync(user);

            return MapToResponse(user);
        }

        public async Task<UserResponse> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null && !user.IsDeleted ? MapToResponse(user) : null;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Where(u => !u.IsDeleted).Select(MapToResponse);
        }

        private string NormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return email.Trim().ToLowerInvariant();
        }

        private string NormalizePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;
            // Basic normalization to E.164 (simplified for demo)
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            if (digits.StartsWith("0") && digits.Length == 11) // BD format 01XXXXXXXXX
            {
                return "+88" + digits;
            }
            return "+" + digits;
        }

        private async Task ValidateUniquenessAsync(string email, string phone)
        {
            var existingEmail = await _userRepository.GetByEmailAsync(email);
            if (existingEmail != null) throw new Exception("Email is already in use.");

            if (!string.IsNullOrEmpty(phone))
            {
                var existingPhone = await _userRepository.GetByPhoneNumberAsync(phone);
                if (existingPhone != null) throw new Exception("Phone number is already in use.");
            }

            var deletedUser = await _userRepository.GetDeletedUserByEmailOrPhoneAsync(email, phone);
            if (deletedUser != null)
            {
                throw new Exception("This account was previously deleted. Please contact an admin for restoration.");
            }
        }

        private void ValidatePasswordPolicy(string password, string email, string phone)
        {
            if (password.Length < 10) throw new Exception("Password must be at least 10 characters long.");
            
            if (!Regex.IsMatch(password, @"[A-Z]")) throw new Exception("Password must contain an uppercase letter.");
            if (!Regex.IsMatch(password, @"[a-z]")) throw new Exception("Password must contain a lowercase letter.");
            if (!Regex.IsMatch(password, @"[0-9]")) throw new Exception("Password must contain a number.");
            if (!Regex.IsMatch(password, @"[\W_]")) throw new Exception("Password must contain a special character.");

            var emailPrefix = email.Split('@')[0];
            if (password.Contains(emailPrefix, StringComparison.OrdinalIgnoreCase))
                throw new Exception("Password cannot contain your email prefix.");

            if (!string.IsNullOrEmpty(phone))
            {
                var last6Digits = phone.Substring(Math.Max(0, phone.Length - 6));
                if (password.Contains(last6Digits))
                    throw new Exception("Password cannot contain your phone number's last 6 digits.");
            }

            if (_weakPasswords.Contains(password, StringComparer.OrdinalIgnoreCase))
                throw new Exception("Password is too weak. Please choose a more secure password.");
        }

        private void ValidateAgePolicy(DateTime? dob)
        {
            if (dob.HasValue)
            {
                var today = DateTime.Today;
                var age = today.Year - dob.Value.Year;
                if (dob.Value.Date > today.AddYears(-age)) age--;

                if (age < 13) throw new Exception("Users must be at least 13 years old to register.");
            }
        }

        private UserResponse MapToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
