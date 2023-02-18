using Domain.Enums;

namespace Domain.Entities;

public class MobileCode
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Code { get; set; }
    public string Token { get; set; }
    public AuthActionType Type { get; set; }
}