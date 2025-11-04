namespace Api.Application.DataTransfer;

public class SystemUserDto
{
    public string? Sub { get; set; }
    public bool? IsActive { get; set; }
    public int? Role { get; set; }
    public string Email { get; set; }
}