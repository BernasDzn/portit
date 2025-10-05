namespace Tests.Domain;

using Api.Domain.Entities;
using Api.Domain.ValueObjects;

public class QualificationsTest
{
    [Theory]
    [InlineData("Q001", "Forklift Operation")]
    [InlineData("Q002", "Crane Operation")]
    [InlineData("Q003", "Hazardous Materials Handling")]
    public void WhenPassingCorrectData_ThenQualificationIsCreated(string code, string designation)
    {
        new Qualification(
            Guid.NewGuid(),
            new Code { Value = code },
            new Designation { Value = designation }
        );
    }

    [Theory]
    [InlineData("", "Forklift Operation")]
    [InlineData("Q002", "")]
    [InlineData("_ asd %", "Crane Operation")]
    public void WhenPassingInvalidData_ThenThrowsException(string code, string designation)
    {
        Assert.Throws<ArgumentException>(() =>
            new Qualification(
                Guid.NewGuid(),
                new Code { Value = code },
                new Designation { Value = designation }
            )
        );
    }
}