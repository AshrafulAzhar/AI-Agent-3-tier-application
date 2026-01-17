using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UserManagement.Domain.DTOs;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Service.Validators
{
    public class PasswordPolicyValidator : IValidator<UserRegistrationRequest>
    {
        private readonly string[] _weakPasswords = { "Password123!", "Admin123!", "Welcome123!" };

        public Task ValidateAsync(UserRegistrationRequest context)
        {
            var password = context.Password;
            var email = context.Email;
            var phone = context.PhoneNumber;

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

            return Task.CompletedTask;
        }
    }
}
