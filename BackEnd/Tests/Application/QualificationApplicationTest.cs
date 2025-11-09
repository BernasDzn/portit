using Api.Application.DataTransfer;
using System.Net.Http.Json;
using Api.Infrastructure.Utilities;

namespace Tests.Application;

public class QualificationApplicationTest : BaseApplicationTest
{
    private readonly HttpClient _client;

    public QualificationApplicationTest()
    {
        _client = Client;
    }

    [Fact]
    public async Task GetAllQualifications_ReturnsQualificationsList()
    {
        var response = await _client.GetAsync("/Qualification");
        response.EnsureSuccessStatusCode();

        var qualifications = await response.Content.ReadFromJsonAsync<List<QualificationDto>>();
        Assert.NotNull(qualifications);
        Assert.NotEmpty(qualifications);
        Assert.Equal(4, qualifications.Count);
    }

    [Fact]
    public async Task FilterQualifications_ReturnsFilteredResults()
    {
        var response = await _client.GetAsync("/Qualification/filter");
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<Page<QualificationDto>>();
        Assert.NotNull(page);
        Assert.NotEmpty(page.Items);
        Assert.Equal(4, page.Items.Count);
    }

    [Fact]
    public async Task FilterQualifications_PageNumberExceedsTotalPages_ReturnsEmptyResult()
    {
        var response = await _client.GetAsync("/Qualification/filter?pageNumber=10&pageSize=2");

        response.EnsureSuccessStatusCode();
        var pagedResult = await response.Content.ReadFromJsonAsync<Page<object>>();
        Assert.NotNull(pagedResult);
        Assert.Empty(pagedResult.Items);
    }

    [Fact]
    public async Task FilterQualifications_ReturnsEmptyResults_WhenNoMatch()
    {
        var response = await _client.GetAsync("/Qualification/filter?qualificationName=NonExistentQualification");
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<Page<QualificationDto>>();
        Assert.NotNull(page);
        Assert.Empty(page.Items);
    }

    [Fact]
    public async Task FilterQualification_ReturnsFiltered_WhenMatchingName()
    {
        var response = await _client.GetAsync("/Qualification/filter?qualificationName=Yard Crane Operator");
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<Page<QualificationDto>>();
        Assert.NotNull(page);
        Assert.Single(page.Items);

        var qualification = page.Items.First();
        Assert.Equal("Yard Crane Operator", qualification.QualificationName);
    }

    [Fact]
    public async Task FilterQualification_ReturnsFiltered_WhenMatchingIdCode()
    {
        var response = await _client.GetAsync("/Qualification/filter?Code=YACOP");
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<Page<QualificationDto>>();
        Assert.NotNull(page);
        Assert.Single(page.Items);

        var qualification = page.Items.First();
        Assert.Equal("YACOP", qualification.IdCode);
    }

    [Fact]
    public async Task FilterQualification_ReturnsEmpty_WhenNoMatchingCodeAndName()
    {
        var response = await _client.GetAsync("/Qualification/filter?Code=STSOP&qualificationName=Yard Crane Operator");
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<Page<QualificationDto>>();
        Assert.NotNull(page);
        Assert.Empty(page.Items);
    }

    [Fact]
    public async Task FilterQualification_ReturnsFiltered_WhenMatchingCodeAndName()
    {
        var response = await _client.GetAsync("/Qualification/filter?Code=YACOP&qualificationName=Yard Crane Operator");
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<Page<QualificationDto>>();
        Assert.NotNull(page);
        Assert.Single(page.Items);

        var qualification = page.Items.First();
        Assert.Equal("YACOP", qualification.IdCode);
        Assert.Equal("Yard Crane Operator", qualification.QualificationName);
    }

    [Fact]
    public async Task FilterQualification_ReturnsPagedResults()
    {
        var response = await _client.GetAsync("/Qualification/filter?pageNumber=1&pageSize=2");
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<Page<QualificationDto>>();
        Assert.NotNull(page);
        Assert.Equal(2, page.Items.Count);
        Assert.Equal(1, page.PageNumber);
        Assert.Equal(2, page.PageSize);
    }

    [Fact]
    public async Task FilterQualification_ReturnsFiltered_WhenInvalidParameterPassed()
    {
        var response = await _client.GetAsync("/Qualification/filter?invalidParam=someValue");
        response.EnsureSuccessStatusCode();

        var page = await response.Content.ReadFromJsonAsync<Page<QualificationDto>>();
        Assert.NotNull(page);
        Assert.Equal(4, page.Items.Count);
    }

    [Fact]
    public async Task GetQualificationById_ReturnsQualification()
    {
        var testId = "STSOP";
        var response = await _client.GetAsync($"/Qualification/{testId}");
        response.EnsureSuccessStatusCode();

        var qualification = await response.Content.ReadFromJsonAsync<QualificationDto>();
        Assert.NotNull(qualification);
        Assert.Equal(testId, qualification.IdCode);
    }

    [Fact]
    public async Task GetQualificationById_ReturnsNotFound_OnNonExistentId()
    {
        var testId = "nonexistentid";
        var response = await _client.GetAsync($"/Qualification/{testId}");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddQualification_ReturnsCreatedQualification()
    {
        var qualificationDto = new QualificationDto
        {
            IdCode = "NEWQUAL",
            QualificationName = "New Qualification"
        };

        var response = await _client.PostAsJsonAsync("/Qualification", qualificationDto);
        response.EnsureSuccessStatusCode();

        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

        var createdQualification = await response.Content.ReadFromJsonAsync<QualificationDto>();
        Assert.NotNull(createdQualification);
        Assert.Equal(qualificationDto.IdCode, createdQualification.IdCode);
        Assert.Equal(qualificationDto.QualificationName, createdQualification.QualificationName);

        // Ensure it was actually added
        var getResponse = await _client.GetAsync($"/Qualification/{qualificationDto.IdCode}");
        getResponse.EnsureSuccessStatusCode();
        var fetchedQualification = await getResponse.Content.ReadFromJsonAsync<QualificationDto>();
        Assert.NotNull(fetchedQualification);
        Assert.Equal(qualificationDto.IdCode, fetchedQualification.IdCode);
        Assert.Equal(qualificationDto.QualificationName, fetchedQualification.QualificationName);
    }

    [Fact]
    public async Task AddQualification_ReturnsConflict_OnDuplicateIdCode()
    {
        var qualificationDto = new QualificationDto
        {
            IdCode = "STSOP",
            QualificationName = "Duplicate Qualification"
        };

        var response = await _client.PostAsJsonAsync("/Qualification", qualificationDto);

        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddQualification_ReturnsBadRequest_OnInvalidData()
    {
        var qualificationDto = new QualificationDto
        {
            IdCode = "",
            QualificationName = "Invalid Qualification"
        };

        var response = await _client.PostAsJsonAsync("/Qualification", qualificationDto);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddQualification_ReturnsBadRequest_OnNullData()
    {
        QualificationDto? qualificationDto = null;

        var response = await _client.PostAsJsonAsync("/Qualification", qualificationDto);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task AddQualification_ReturnsBadRequest_OnWrongFormatData()
    {
        var wrongFormatData = new
        {
            WrongField = "SomeValue"
        };

        var response = await _client.PostAsJsonAsync("/Qualification", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQualification_ReturnsNoContent()
    {
        var qualificationDto = new QualificationDto
        {
            IdCode = "STSOP",
            QualificationName = "Updated STS Operator"
        };

        var response = await _client.PutAsJsonAsync($"/Qualification/{qualificationDto.IdCode}", qualificationDto);
        response.EnsureSuccessStatusCode();

        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update
        var getResponse = await _client.GetAsync($"/Qualification/{qualificationDto.IdCode}");
        getResponse.EnsureSuccessStatusCode();

        var updatedQualification = await getResponse.Content.ReadFromJsonAsync<QualificationDto>();
        Assert.NotNull(updatedQualification);
        Assert.Equal(qualificationDto.QualificationName, updatedQualification.QualificationName);
    }

    [Fact]
    public async Task UpdateQualification_ReturnsNotFound_OnNonExistentId()
    {
        var qualificationDto = new QualificationDto
        {
            IdCode = "NONEXISTENT",
            QualificationName = "Non Existent Qualification"
        };

        var response = await _client.PutAsJsonAsync($"/Qualification/{qualificationDto.IdCode}", qualificationDto);

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQualification_ReturnsBadRequest_OnInvalidData()
    {
        var qualificationDto = new QualificationDto
        {
            IdCode = "",
            QualificationName = ""
        };

        var response = await _client.PutAsJsonAsync($"/Qualification/STSOP", qualificationDto);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQualification_ReturnsBadRequest_OnNullData()
    {
        QualificationDto? qualificationDto = null;

        var response = await _client.PutAsJsonAsync($"/Qualification/ANYID", qualificationDto);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateQualification_ReturnsBadRequest_OnWrongFormatData()
    {
        var wrongFormatData = new
        {
            WrongField = "SomeValue"
        };

        var response = await _client.PutAsJsonAsync($"/Qualification/ANYID", wrongFormatData);

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}