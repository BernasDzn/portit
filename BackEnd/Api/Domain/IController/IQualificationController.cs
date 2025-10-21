using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers;

public interface IQualificationController
{
    public Task<ActionResult<IEnumerable<QualificationDto>>> GetAll();
    public Task<ActionResult<Page<QualificationDto>>> Filter([FromQuery] QualificationFilter filter);
    public Task<ActionResult<QualificationDto>> GetById(string id);
    public Task<ActionResult<QualificationDto>> Create(QualificationDto qualificationDto);
    public Task<ActionResult> Update(string id, QualificationDto qualificationDto);
}