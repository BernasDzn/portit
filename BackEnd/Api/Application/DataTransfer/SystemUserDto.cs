namespace Api.Application.DataTransfer;

public class SystemUserDto
{
    public string Sub { get; set; }
    public bool IsActive { get; set; }
    public string[] Roles { get; set; }
}