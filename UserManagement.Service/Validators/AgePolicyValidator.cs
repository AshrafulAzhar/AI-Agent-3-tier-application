using System;
using System.Threading.Tasks;
using UserManagement.Domain.DTOs;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Service.Validators
{
    public class AgePolicyValidator : IValidator<UserRegistrationRequest>
    {
        public Task ValidateAsync(UserRegistrationRequest context)
        {
            var dob = context.DateOfBirth;
            if (dob.HasValue)
            {
                var today = DateTime.Today;
                var age = today.Year - dob.Value.Year;
                if (dob.Value.Date > today.AddYears(-age)) age--;

                if (age < 13) throw new Exception("Users must be at least 13 years old to register.");
            }
            return Task.CompletedTask;
        }
    }
}
