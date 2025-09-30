namespace Application.Services;

using Api.Domain.Model;
using Domain.IRepository;
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

	public async Task<QualificationDto> GetQualificationByName(string name, List<string> errorMessage)
	{
		Qualification qualification = await _qualificationRepository.GetQualificationByNameAsync(name);
		if (qualification == null)
		{
			errorMessage.Add("Qualification not found.");
			return null;
		}
		return qualification.ToDTO();
	}

	public async Task<QualificationDto> Add(QualificationDto qualificationDto, List<string> errorMessage)
	{
		bool exists = await _qualificationRepository.QualificationExists(qualificationDto.QualificationName);
		if (exists)
		{
			errorMessage.Add("Qualification with the same ID already exists.");
			return null;
		}

		Qualification qualification = QualificationDto.ToDomain(qualificationDto);
		Qualification savedQualification = await _qualificationRepository.Add(qualification);
		QualificationDto savedQualificationDto = savedQualification.ToDTO();

		return savedQualificationDto;
	}

	public async Task<bool> Update(Guid id, QualificationDto qualificationDto, List<string> errorMessage)
	{
		Qualification qualification = await _qualificationRepository.GetQualificationByNameAsync(qualificationDto.QualificationName);

		if (qualification == null)
		{
			errorMessage.Add("Qualification not found.");
			return false;
		}

		qualification.UpdateQualificationName(qualificationDto.QualificationName);
		await _qualificationRepository.Update(qualification, errorMessage);
		return true;
		
	}

}