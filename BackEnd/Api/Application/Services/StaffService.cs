namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Application.Exceptions;
using Api.Domain.Entities;
using Api.Domain.IRepository;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Exceptions;
using Api.Infrastructure.Utilities;

public class StaffService : IStaffService
{

	private readonly IStaffRepository _staffRepository;
	private readonly IQualificationRepository _qualificationRepository;
	private readonly ILogger<StaffService> _logger;

	public StaffService(IStaffRepository staffRepository, IQualificationRepository qualificationRepository, ILogger<StaffService> logger)
	{
		_staffRepository = staffRepository;
		_qualificationRepository = qualificationRepository;
		_logger = logger;
	}

	public async Task<IEnumerable<StaffDto>> GetStaffs()
	{
		var staffs = await _staffRepository.GetStaffsAsync();
		AppLogEvents.LogRetrieve(_logger, "staffs", staffs.Count());
		return staffs.Select(s => s.ToDTO()).ToList();
	}

	public async Task<StaffDto?> Add(CreateStaffDto staffDto)
	{
		bool exists = await _staffRepository.GetStaffByMecNumberAsync(staffDto.MechanograficNumber) != null;
		if (exists)
			throw new EntityAlreadyExistsException("Staff with the same mechanographic number already exists.");

		ICollection<Qualification> qualifications = new List<Qualification>();
		if (staffDto.QualificationsCodes != null)
		{
			foreach (var qualificationCode in staffDto.QualificationsCodes)
			{
				var qual = await _qualificationRepository.GetQualificationByIdAsync(qualificationCode);
				if (qual == null)
					throw new EntityNotFoundException($"Qualification with id {qualificationCode} not found.");
				qualifications.Add(qual);
			}
		}

		Staff staff = new Staff(
			new StaffMechanograficNumber { Value = staffDto.MechanograficNumber },
			new Designation { Value = staffDto.Name },
			new Email { Value = staffDto.Email },
			new PhoneNumber { Value = staffDto.PhoneNumber },
            staffDto.OperationalWindow,
			qualifications
		);
		await _staffRepository.Add(staff);
		Staff addedStaff = await _staffRepository.GetStaffByMecNumberAsync(staffDto.MechanograficNumber) ?? throw new Exception("Error retrieving the added staff.");
		StaffDto addedStaffDto = addedStaff.ToDTO();

		//_logger.LogInformation("Staff {StaffId} created.", addedStaff.Id);
		AppLogEvents.LogCreate(_logger, "Staff", addedStaffDto.MechanograficNumber);
		return addedStaffDto;
	}

	public async Task<StaffDto?> Update(string mecanographicNumber, CreateStaffDto staffDto)
	{
		Staff? staff = await _staffRepository.GetStaffByMecNumberAsync(mecanographicNumber);
		if (staff == null)
			throw new EntityNotFoundException("Staff not found.");

		HashSet<Qualification> qualifications = new HashSet<Qualification>();
		if (staffDto.QualificationsCodes != null)
		{
			foreach (var qualificationCode in staffDto.QualificationsCodes)
			{
				var qual = await _qualificationRepository.GetQualificationByIdAsync(qualificationCode);
				if (qual == null)
					throw new EntityNotFoundException($"Qualification with id {qualificationCode} not found.");
				qualifications.Add(qual);
			}
		}

		staff.Update(
			staffDto.Name,
			staffDto.Email,
			staffDto.PhoneNumber,
			staffDto.Status,
			staffDto.OperationalWindow,
			qualifications
		);

		//_logger.LogInformation("Staff {StaffId} updated.", staff.Id);
		AppLogEvents.LogUpdate(_logger, "Staff", staff.Id);
		return (await _staffRepository.Update(staff)).ToDTO();
	}

	public async Task<Page<StaffDto>> FilterStaffs(StaffFilter staffFilter)
	{
		Page<Staff> page = await _staffRepository.FilterStaffsAsync(staffFilter);
		AppLogEvents.LogFilter(_logger, "staffs", page.Items.Count);
		return page.Map<StaffDto>(s => s.ToDTO());

	}

	public async Task<StaffDto?> Deactivate(string mecanographicNumber)
	{
		var staff = await _staffRepository.GetStaffByMecNumberAsync(mecanographicNumber);
		if (staff == null)
			throw new EntityNotFoundException("Staff not found.");

		staff.Deactivate();
		await _staffRepository.Update(staff);

		//_logger.LogInformation("Staff {StaffId} deactivated.", staff.Id);
		AppLogEvents.LogDeactivate(_logger, "Staff", staff.Id);
		return staff.ToDTO();
	}

}