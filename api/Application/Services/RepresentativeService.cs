namespace Application.Services;

using Api.Application.Exceptions;
using Api.Domain.Model;
using Api.Models;
using Domain.IRepository;
using Domain.Model.Generic;
using Microsoft.EntityFrameworkCore;

public class RepresentativeService
{
    private readonly IRepresentativeRepository _representativeRepository;

    public RepresentativeService(IRepresentativeRepository representativeRepository)
    {
        _representativeRepository = representativeRepository;
    }

    public async Task<IEnumerable<RepresentativeDto>> GetRepresentatives()
    {
        var representatives = await _representativeRepository.GetAllAsync();
        return representatives.Select(r => r.ToDTO()).ToList();
    }

    public async Task<RepresentativeDto> GetRepresentativeByEmail(string email)
    {
        var representative = await _representativeRepository.GetByEmailAsync(email);
        if (representative == null)
            throw new EntityNotFoundException("Representative not found.");

        return representative.ToDTO();
    }

    public async Task<RepresentativeDto> GetRepresentativeByCitizenId(string citizenId)
    {
        var representative = await _representativeRepository.GetByCitizenIdAsync(citizenId);
        if (representative == null)
            throw new EntityNotFoundException("Representative not found.");

        return representative.ToDTO();
    }
}