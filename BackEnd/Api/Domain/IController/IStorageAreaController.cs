using Api.Application.DataTransfer;
using Api.Application.DataTransfer.Filters;
using Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Application.Controllers;

public interface IStorageAreaController
{
	Task<ActionResult<IEnumerable<StorageAreaDto>>> GetAll();
	Task<ActionResult<StorageAreaDto>> Get(string id);
	Task<ActionResult<StorageAreaDto>> Create(CreateStorageAreaDto createStorageAreaDto);
	Task<ActionResult<StorageAreaDto>> Update(string id, [FromBody] CreateStorageAreaDto updateStorageAreaDto);
}
