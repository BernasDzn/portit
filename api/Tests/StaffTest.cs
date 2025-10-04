using Api.Domain.Model;
using Xunit;

namespace api.Tests;

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