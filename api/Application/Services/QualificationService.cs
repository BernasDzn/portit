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

	public async Task<QualificationDto?> GetQualificationByName(string name, List<string> errorMessage)
	{
		Qualification qualification = await _qualificationRepository.GetQualificationByNameAsync(name);
		if (qualification == null)
		{
			errorMessage.Add("Qualification not found.");
			return null;
		}
		return qualification.ToDTO();
	}

	public async Task<QualificationDto?> Add(QualificationDto qualificationDto)
	{
		bool exists = await _qualificationRepository.GetQualificationByNameAsync(qualificationDto.QualificationName) != null;

		if (exists)
			throw new EntityAlreadyExistsException("Qualification with the same name already exists.");

		Qualification qualification = new Qualification(
			Guid.NewGuid(),
			new Designation { Value = qualificationDto.QualificationName }
		);

		Qualification savedQualification = await _qualificationRepository.Add(qualification);
		QualificationDto savedQualificationDto = savedQualification.ToDTO();

		return savedQualificationDto;
	}

	public async Task<QualificationDto?> Update(string name, QualificationDto qualificationDto)
	{
		Qualification qualification = await _qualificationRepository.GetQualificationByNameAsync(name);
		if (qualification == null)
			throw new EntityNotFoundException("Qualification to update not found.");

		qualification.UpdateQualificationName(qualificationDto.QualificationName);

		Qualification? updateResult = await _qualificationRepository.Update(qualification);
		if (updateResult == null)
			throw new EntityNotFoundException("Qualification to update not found.");

		return updateResult.ToDTO();
	}
}