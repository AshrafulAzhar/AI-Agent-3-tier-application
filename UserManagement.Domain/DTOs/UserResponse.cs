using System;
using UserManagement.Domain.Enums;

namespace UserManagement.Domain.DTOs
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
