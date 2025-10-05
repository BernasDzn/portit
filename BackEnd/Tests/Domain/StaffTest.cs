namespace Tests.Domain;

using Api.Domain.ValueObjects;

public class StaffTest
{

	[Theory]
	[InlineData("")]
	public void WhenPassingInvalidMecanographicNumber_ThenThrowsException(string mecanographicNumber)
	{
		Assert.Throws<ArgumentException>(() =>
			new StaffMechanograficNumber { Value = mecanographicNumber }
		);
	}

}