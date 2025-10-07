namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;

public class QualificationService
{
	private readonly IQualificationRepository _qualificationRepository;
	private readonly ILogger<QualificationService> _logger;
	public QualificationService(IQualificationRepository qualificationRepository, ILogger<QualificationService> logger)
	{
		_qualificationRepository = qualificationRepository;
		_logger = logger;
	}

	public async Task<IEnumerable<QualificationDto>> GetQualifications()
	{
		var qualifications = await _qualificationRepository.GetQualificationsAsync();		
		return qualifications.Select(q => q.ToDTO()).ToList();
	}

	public async Task<QualificationDto?> GetQualificationById(string id)
	{
		var qualification = await _qualificationRepository.GetQualificationByIdAsync(id);
		if (qualification == null)
			return null;

		return qualification.ToDTO();
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

		_logger.LogInformation("Qualification {QualificationId} created.", savedQualification.Id);
		return savedQualificationDto;
	}

	public async Task<QualificationDto?> Update(string id, QualificationDto qualificationDto)
	{
		Qualification? qualification = await _qualificationRepository.GetQualificationByIdAsync(id);
		if (qualification == null)
			throw new EntityNotFoundException("Qualification to update not found.");

		qualification.UpdateQualificationName(qualificationDto.QualificationName);

		Qualification? updateResult = await _qualificationRepository.Update(qualification);
		if (updateResult == null)
			throw new EntityNotFoundException("Qualification to update not found.");

		_logger.LogInformation("Qualification {QualificationId} updated.", updateResult.Id);
		return updateResult.ToDTO();
	}

	public async Task<Page<QualificationDto>> FilterQualifications(QualificationFilter filter)
	{
		Page<Qualification> page = await _qualificationRepository.FilterQualificationsAsync(filter);
		return page.Map<QualificationDto>(q => q.ToDTO());
	}
}