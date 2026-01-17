using System;

namespace UserManagement.Domain.DTOs
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
