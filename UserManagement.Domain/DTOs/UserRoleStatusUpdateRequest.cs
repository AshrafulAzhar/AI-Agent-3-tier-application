using UserManagement.Domain.Enums;

namespace UserManagement.Domain.DTOs
{
    public class UserRoleStatusUpdateRequest
    {
        public UserRole? Role { get; set; }
        public UserStatus? Status { get; set; }
    }
}
