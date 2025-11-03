namespace Tests.Unitary.Domain;

using Api.Domain.Entities;
using Api.Domain.ValueObjects;

public class StaffTest
{

	OperationalWindow validOperationalWindow = OperationalWindow.FullWeek();

	[Theory]
	[InlineData("AB@42&")]
	public void WhenPassingNonAlphanumericMecanographicNumber_ThenThrowsException(string mecanographicNumber)
	{
		Assert.Throws<ArgumentException>(() =>
			new StaffMechanographicNumber { Value = mecanographicNumber }
		);
	}

	[Theory]
	[InlineData("")]
	public void WhenPassingInvalidMecanographicNumber_ThenThrowsException(string mecanographicNumber)
	{
		Assert.Throws<ArgumentException>(() =>
			new StaffMechanographicNumber { Value = mecanographicNumber }
		);
	}

	[Theory]
	[InlineData("123456789", "Staff Name", "staff@example.com", "910000000")]
	[InlineData("MEC001", "Another Staff", "another@example.com", "920000000")]
	[InlineData("EMP123", "Test Staff", "test@example.com", "930000000")]
	public void WhenCreatingStaffWithoutQualification_ThenNotThrowsException(
		string mecanographicNumber,
		string name,
		string email,
		string phoneNumber)
	{
		Staff staff = new Staff(
			new StaffMechanographicNumber { Value = mecanographicNumber },
			new Designation { Value = name },
			new Email { Value = email },
			new PhoneNumber { Value = phoneNumber },
			validOperationalWindow,
			new List<Qualification>()
		);
	}

	[Fact]
	public void UpdateStatus_WithValidStatus_UpdatesSuccessfully()
	{
		var staff = new Staff(
			new StaffMechanographicNumber { Value = "MEC001" },
			new Designation { Value = "Name" },
			new Email { Value = "a@b.com" },
			new PhoneNumber { Value = "900000000" },
			validOperationalWindow,
			new List<Qualification>()
		);

		staff.UpdateStatus((int)StaffStatus.TemporarilyReassigned);
		Assert.Equal(StaffStatus.TemporarilyReassigned, staff.Status);
	}

	[Fact]
	public void UpdateStatus_WithInvalidStatus_ThrowsArgumentException()
	{
		var staff = new Staff(
			new StaffMechanographicNumber { Value = "MEC002" },
			new Designation { Value = "Name" },
			new Email { Value = "a@b.com" },
			new PhoneNumber { Value = "900000000" },
			validOperationalWindow,
			new List<Qualification>()
		);

		Assert.Throws<ArgumentException>(() => staff.UpdateStatus(999));
	}

	[Fact]
	public void Deactivate_SetsStatusUnavailableAndIsActiveFalse()
	{
		var staff = new Staff(
			new StaffMechanographicNumber { Value = "MEC003" },
			new Designation { Value = "Name" },
			new Email { Value = "a@b.com" },
			new PhoneNumber { Value = "900000000" },
			validOperationalWindow,
			new List<Qualification>()
		);

		staff.Deactivate();
		Assert.Equal(StaffStatus.Unavailable, staff.Status);
		Assert.False(staff.isActive);
	}

	[Fact]
	public void Update_ChangesPropertiesCorrectly()
	{
		var staff = new Staff(
			new StaffMechanographicNumber { Value = "MEC004" },
			new Designation { Value = "OldName" },
			new Email { Value = "old@a.com" },
			new PhoneNumber { Value = "900000001" },
			validOperationalWindow,
			new List<Qualification>()
		);

		var newQual = new Qualification(new Guid(), new Code { Value = "Q1" }, new Designation { Value = "Qualification" });
		var qualifications = new HashSet<Qualification> { newQual };

		staff.Update("NewName", "new@a.com", "900000002", (int)StaffStatus.Available, OperationalWindow.FullWeek(), qualifications);

		Assert.Equal("NewName", staff.Name.Value);
		Assert.Equal("new@a.com", staff.Email.Value);
		Assert.Equal("900000002", staff.PhoneNumber.Value);
		Assert.Equal(StaffStatus.Available, staff.Status);
	}

	[Fact]
	public void ToDTO_ReturnsCorrectDto()
	{
		var staff = new Staff(
			new StaffMechanographicNumber { Value = "MEC005" },
			new Designation { Value = "Name" },
			new Email { Value = "a@b.com" },
			new PhoneNumber { Value = "900000000" },
			validOperationalWindow,
			new List<Qualification>()
		);

		var dto = staff.ToDTO();
		Assert.Equal(staff.MechanographicNumber.Value, dto.MechanographicNumber);
		Assert.Equal(staff.Name.Value, dto.Name);
	}

	[Fact]
	public void ToString_IncludesMechanograficNumber()
	{
		var staff = new Staff(
			new StaffMechanographicNumber { Value = "MEC006" },
			new Designation { Value = "Name" },
			new Email { Value = "a@b.com" },
			new PhoneNumber { Value = "900000000" },
			validOperationalWindow,
			new List<Qualification>()
		);

		var str = staff.ToString();
		Assert.Contains("MEC006", str);
	}

	

}