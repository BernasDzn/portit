using Api.Domain.Model;

public class QualificationDto
{
    public required string QualificationName { get; set; }

    public static Qualification ToDomain(QualificationDto dto)
    {
        Qualification qualification = new Qualification(dto.QualificationName);
        return qualification;
    }
}