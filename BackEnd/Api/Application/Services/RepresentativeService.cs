namespace Api.Application.Services;

using Api.Application.DataTransfer;
using Api.Application.Exceptions;
using Api.Domain.IRepository;

public class RepresentativeService
{
    private readonly IRepresentativeRepository _representativeRepository;
    private readonly ILogger<RepresentativeService> _logger;

    public RepresentativeService(IRepresentativeRepository representativeRepository, ILogger<RepresentativeService> logger)
    {
        _representativeRepository = representativeRepository;
        _logger = logger;
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

    public async Task<RepresentativeDto> GetRepresentativeByCitizenId(uint citizenId)
    {
        var representative = await _representativeRepository.GetByCitizenIdAsync(citizenId);
        if (representative == null)
            throw new EntityNotFoundException("Representative not found.");

        return representative.ToDTO();
    }
}