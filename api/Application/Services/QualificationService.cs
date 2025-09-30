namespace Application.Services;

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

	public async Task<QualificationDto?> Add(QualificationDto qualificationDto, List<string> errorMessage)
	{
		bool exists = await _qualificationRepository.GetQualificationByNameAsync(qualificationDto.QualificationName) != null;

		if (exists)
		{
			errorMessage.Add("Qualification with the same name already exists.");
			return null;
		}

		Qualification qualification = new Qualification(new Designation { Value = qualificationDto.QualificationName });
		Qualification savedQualification = await _qualificationRepository.Add(qualification);
		QualificationDto savedQualificationDto = savedQualification.ToDTO();

		return savedQualificationDto;
	}

	public async Task<QualificationDto?> Update(string name, QualificationDto qualificationDto, List<string> errorMessage)
	{
		bool updateResult = await _qualificationRepository.Update(name, qualificationDto, errorMessage);
		if (!updateResult)
		{
			return null;
		}

		return await GetQualificationByName(qualificationDto.QualificationName, errorMessage);
	}
}