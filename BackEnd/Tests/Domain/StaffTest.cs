namespace Tests.Domain;

using Api.Domain.Entities;
using Api.Domain.ValueObjects;

public class StaffTest
{

	OperationalWindow validOperationalWindow = new OperationalWindow
	{
		StartWeekDay = DayOfWeek.Monday,
		EndWeekDay = DayOfWeek.Friday,
		DayStartTime = new TimeOnly(9, 0),
		DayEndTime = new TimeOnly(17, 0)
	};

	[Theory]
	[InlineData("AB@42&")]
	public void WhenPassingNonAlphanumericMecanographicNumber_ThenThrowsException(string mecanographicNumber)
	{
		Assert.Throws<ArgumentException>(() =>
			new StaffMechanograficNumber { Value = mecanographicNumber }
		);
	}

	[Theory]
	[InlineData("")]
	public void WhenPassingInvalidMecanographicNumber_ThenThrowsException(string mecanographicNumber)
	{
		Assert.Throws<ArgumentException>(() =>
			new StaffMechanograficNumber { Value = mecanographicNumber }
		);
	}

	[Theory]
	[InlineData("123456789", "Staff Name", "staff@example.com", "910000000")]
	public void WhenCreatingStaffWithoutQualification_ThenNotThrowsException(
		string mecanographicNumber,
		string name,
		string email,
		string phoneNumber)
	{
		Staff staff = new Staff(
			new StaffMechanograficNumber { Value = mecanographicNumber },
			new Designation { Value = name },
			new Email { Value = email },
			new PhoneNumber { Value = phoneNumber },
			validOperationalWindow,
			new List<Qualification>()
		);
	}

}