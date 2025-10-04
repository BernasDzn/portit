using System.Net.Http.Headers;
using Api.Application.Exceptions;
using Api.Domain.Model;
using Api.Representations.DTO;
using Domain.IRepository;
using Domain.Model.Generic;

namespace Application.Services;

public class StaffService
{

	private readonly IStaffRepository _staffRepository;
	private readonly IQualificationRepository _qualificationRepository;

	public StaffService(IStaffRepository staffRepository, IQualificationRepository qualificationRepository)
	{
		_staffRepository = staffRepository;
		_qualificationRepository = qualificationRepository;
	}

	public async Task<IEnumerable<StaffDto>> GetStaffs()
	{
		var staffs = await _staffRepository.GetStaffsAsync();
		return staffs.Select(s => s.ToDTO()).ToList();
	}

	public async Task<StaffDto?> Add(StaffDto staffDto)
	{
		bool exists = await _staffRepository.GetStaffByMecNumberAsync(staffDto.MechanograficNumber) != null;
		if (exists)
			throw new EntityAlreadyExistsException("Staff with the same mechanographic number already exists.");

		ICollection<Qualification> qualifications = new List<Qualification>();
		if (staffDto.Qualifications != null)
		{
			foreach (var qualification in staffDto.Qualifications)
			{
				var qual = await _qualificationRepository.GetQualificationByIdAsync(qualification.IdCode);
				if (qual == null)
					throw new EntityNotFoundException($"Qualification with id {qualification.IdCode} not found.");
				qualifications.Add(qual);
			}
		}

		Staff staff = new Staff(
			new StaffMechanograficNumber { Value = staffDto.MechanograficNumber },
			new Designation { Value = staffDto.Name },
			new Email { Value = staffDto.Email },
			new PhoneNumber { Value = staffDto.PhoneNumber },
			new OperationalWindow
			{
				StartWeekDay = staffDto.OperationalWindow.StartWeekDay,
				EndWeekDay = staffDto.OperationalWindow.EndWeekDay,
				DayStartTime = staffDto.OperationalWindow.DayStartTime,
				DayEndTime = staffDto.OperationalWindow.DayEndTime
			},
			qualifications
		);
		await _staffRepository.Add(staff);
		Staff addedStaff = await _staffRepository.GetStaffByMecNumberAsync(staffDto.MechanograficNumber) ?? throw new Exception("Error retrieving the added staff.");
		StaffDto addedStaffDto = addedStaff.ToDTO();
		
		return addedStaffDto;
	}

}