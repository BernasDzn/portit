namespace Api.Domain.IController;

using Api.Application.DataTransfer;
using Microsoft.AspNetCore.Mvc;

public interface IPrivacyPolicyController
{
	Task<ActionResult<PrivacyPolicyDto>> GetActive();
	Task<ActionResult<IEnumerable<PrivacyPolicyDto>>> GetAll();
	Task<ActionResult<PrivacyPolicyDto>> GetById(Guid id);
	Task<ActionResult<PrivacyPolicyDto>> Create(CreatePrivacyPolicyDto createDto);
}
