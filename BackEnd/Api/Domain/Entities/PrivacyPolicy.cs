namespace Api.Domain.Entities;

using Api.Domain.ValueObjects;
using Api.Application.DataTransfer;
using Api.Infrastructure.Utilities;

public class PrivacyPolicy : IDTOAble<PrivacyPolicyDto>
{
	public Guid Id { get; private set; }
	public string Content { get; private set; }
	public DateTime UpdatedOn { get; private set; }
	public bool Active { get; private set; } = true; // Soft delete

	protected PrivacyPolicy() { } // EF Core

	internal PrivacyPolicy(Guid id, string content, DateTime updatedOn, bool active)
	{
		Id = id;
		Content = content ?? throw new ArgumentNullException(nameof(content));
		UpdatedOn = updatedOn;
		Active = active;
	}

	public void Deactivate()
	{
		Active = false;
	}

	public PrivacyPolicy Update(string content)
	{
		UpdateContent(content);
		UpdateUpdatedOn(DateTime.UtcNow);
		return this;
	}

	private void UpdateContent(string content) { Content = content; }
	private void UpdateUpdatedOn(DateTime updatedOn) { UpdatedOn = updatedOn; }

	public PrivacyPolicyDto ToDTO()
	{
		return new PrivacyPolicyDto
		{
			Id = Id,
			Content = Content,
			UpdatedOn = UpdatedOn,
			Active = Active
		};
	}
}
