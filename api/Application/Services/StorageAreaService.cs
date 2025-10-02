namespace Application.Services;

using Api.Application.Exceptions;
using Api.Domain.Model;
using Domain.IRepository;
using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;

public class StorageAreaService
{
    private readonly IStorageAreaRepository _storageAreaRepository;

    public StorageAreaService(IStorageAreaRepository qualificationRepository)
    {
        _storageAreaRepository = qualificationRepository;
    }

    public async Task<IEnumerable<StorageAreaDto>> GetStorageAreas()
    {
        var qualifications = await _storageAreaRepository.GetStorageAreasAsync();
        return qualifications.Select(q => q.ToDTO()).ToList();
	}

	public async Task<StorageAreaDto?> GetStorageAreaByCode(string name)
	{
		StorageArea? qualification = await _storageAreaRepository.GetStorageAreaByCodeAsync(name);
        if (qualification == null)
            throw new EntityNotFoundException("Storage area not found.");

        return qualification.ToDTO();
	}
}