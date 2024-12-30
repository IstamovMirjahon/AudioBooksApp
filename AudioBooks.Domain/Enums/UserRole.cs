using System.ComponentModel;

namespace AudioBooks.Domain.Enums;

public enum UserRole
{
    [Description("SuperAdmin")]
    SuperAdmin = 1,
    [Description("User")]
    User = 2,
}
