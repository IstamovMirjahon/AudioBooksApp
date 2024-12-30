using System.ComponentModel;

namespace AudioBooks.Domain.Enums;

public enum UserStatus
{
    [Description("Block")]
    Block = 0,
    [Description("Pending")]
    Pending = 1,
    [Description("Active")]
    Active = 2,
}
