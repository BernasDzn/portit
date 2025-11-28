namespace Api.Application.DataTransfer;

using Api.Domain.Entities;

public class PrivacyPolicyDto
{
	public Guid Id { get; set; }
	public string Content { get; set; }
	public DateTime UpdatedOn { get; set; }
	public bool Active { get; set; }
}