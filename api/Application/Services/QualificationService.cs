namespace Application.Services;

using Api.Application.Exceptions;
using Api.Domain.Model;
using Domain.IRepository;
using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;

public class QualificationService
{
	private readonly IQualificationRepository _qualificationRepository;

	public QualificationService(IQualificationRepository qualificationRepository)
	{
		_qualificationRepository = qualificationRepository;
	}

	public async Task<IEnumerable<QualificationDto>> GetQualifications()
	{
		var qualifications = await _qualificationRepository.GetQualificationsAsync();
		return qualifications.Select(q => q.ToDTO()).ToList();
	}

	public async Task<QualificationDto?> Add(QualificationDto qualificationDto)
	{
		bool exists = await _qualificationRepository.GetQualificationByIdAsync(qualificationDto.IdCode) != null;

		if (exists)
			throw new EntityAlreadyExistsException("Qualification with the same name already exists.");

		Qualification qualification = new Qualification(
			Guid.NewGuid(),
			new Code { Value = qualificationDto.IdCode },
			new Designation { Value = qualificationDto.QualificationName }
		);

		Qualification savedQualification = await _qualificationRepository.Add(qualification);
		QualificationDto savedQualificationDto = savedQualification.ToDTO();

		return savedQualificationDto;
	}

	public async Task<QualificationDto?> Update(string id, QualificationDto qualificationDto)
	{
		Qualification? qualification = await _qualificationRepository.GetQualificationByIdAsync(id);
		if (qualification == null)
			throw new EntityNotFoundException("Qualification to update not found.");

		qualification.UpdateQualificationName(qualificationDto.QualificationName);
		qualification.UpdateIdCode(qualificationDto.IdCode);

		Qualification? updateResult = await _qualificationRepository.Update(qualification);
		if (updateResult == null)
			throw new EntityNotFoundException("Qualification to update not found.");

		return updateResult.ToDTO();
	}

	public async Task<Page<QualificationDto>> FilterQualifications(QualificationFilter filter)
	{
		Page<Qualification> page = await _qualificationRepository.FilterQualificationsAsync(filter);
		return page.Map<QualificationDto>(q => q.ToDTO());
	}
}