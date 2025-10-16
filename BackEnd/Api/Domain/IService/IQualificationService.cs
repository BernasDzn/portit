using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;

namespace Api.Application.Services;

public interface IQualificationService
{
    Task<IEnumerable<QualificationDto>> GetQualifications();
    Task<QualificationDto> GetQualificationById(string id);
    Task<QualificationDto> Add(QualificationDto qualificationDto);
    Task<QualificationDto> Update(string id, QualificationDto qualificationDto);
    Task<Page<QualificationDto>> FilterQualifications(QualificationFilter filter);
}