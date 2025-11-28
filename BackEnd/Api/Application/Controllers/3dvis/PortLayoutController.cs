using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;
using Api.Domain.Entities;

namespace Api.Application.Controllers;

[ApiController]
[Route("[controller]")]
public class PortLayoutController : ControllerBase
{
	private readonly IDockService _dockService;
	private readonly IStorageAreaService _storageAreaService;
	private readonly IPhysicalResourceService _physicalResourceService;
	private readonly ILogger<PortLayoutController> _logger;
	private readonly IVesselVisitNotificationService _vvnService;

	public PortLayoutController(
		IDockService dockService,
		IStorageAreaService storageAreaService,
		IPhysicalResourceService physicalResourceService,
		IVesselVisitNotificationService vvnService,
		ILogger<PortLayoutController> logger)
	{
		_dockService = dockService;
		_storageAreaService = storageAreaService;
		_physicalResourceService = physicalResourceService;
		_vvnService = vvnService;
		_logger = logger;
	}

	[HttpGet("/VesselPositions", Name = "GetVesselPositions")]
	public async Task<ActionResult<IEnumerable<VesselPositionDto>>> GetVesselPositions()
	{
		try
		{
			var vesselPositions = await _vvnService.GetVesselPositionsAsync();
			return Ok(vesselPositions);
		}
		catch (Exception e)
		{
			_logger.LogError("Error fetching vessel positions, {Message}", e.Message);
			return StatusCode(500, "An error occurred while fetching vessel positions.");
		}
	}

	[HttpGet("/PortLayout", Name = "GetPortLayout")]
	public async Task<ActionResult<IEnumerable<PortChunk>>> GetPortLayout()
	{
		try
		{
			var userRole = User.Claims.FirstOrDefault(c => c.Type == "user_role")?.Value ?? "Guest";
			bool showDetails = userRole == "PortAuthorityOfficer" || userRole == "Administrator" || userRole == "LogisticsOperator";

			var chunks = new List<PortChunk>();

			// Fetch all required data
			var storageAreas = await _storageAreaService.GetStorageAreas();
			var docks = await _dockService.GetDocks();
			var resources = await _physicalResourceService.GetPhysicalResources();

			// Grid configuration
			const int gridWidth = 10;
			
			// First 2 rows: Warehouses and Yards (column by column)
            var storageAreasList = storageAreas.ToList();
			int storageIndex = 0;

			for (int col = 0; col < gridWidth; col++)
			{
				for (int row = 0; row < 2; row++)
				{
					if (storageIndex < storageAreasList.Count)
					{
						var storage = storageAreasList[storageIndex];
						var chunkType = storage.Type == StorageAreaType.Warehouse
							? ChunkType.Warehouse
							: ChunkType.Yard;

						object? details = null;
						if (showDetails)
						{
							// Format the storage area details
							var dockServicesList = (storage.DockServices == null || storage.DockServices.Count == 0) && storage.Type == StorageAreaType.Yard
								? "All docks"
								: string.Join(", ", storage.DockServices.Select(ds => ds.Dock.Name));

							details = new
							{
								NameCode = storage.NameCode,
								Location = storage.Location,
								Type = storage.Type.ToString(),
								Capacity = storage.Capacity,
								CurrentOccupancy = storage.CurrentOccupancy,
								DockServices = dockServicesList
							};
						}

						chunks.Add(new PortChunk(
							storage.NameCode,
							chunkType,
							col,
							row,
							new { 
								title = storage.NameCode, 
								description = storage.Type.ToString(),
								details = details
							}
						));
						storageIndex++;
					}
					else
					{
						chunks.Add(new PortChunk(
							$"Land_{col}_{row}",
							ChunkType.Land,
							col,
							row
						));
					}
				}
			}

			// Rows 2-3: Docks in pairs with gaps (columns 0-1, 3-4, 6-7, 9-10)
			var docksList = docks.ToList();
			int dockIndex = 0;
			var dockColumns = new[] { 0, 2, 4, 6, 8 };

			foreach (var baseCol in dockColumns)
			{
				if (dockIndex >= docksList.Count) break;

			// Place 2 docks vertically at this column position
			for (int row = 2; row < 4 && dockIndex < docksList.Count; row++)
			{
				var dock = docksList[dockIndex];
				
				object? details = null;
				if (showDetails)
				{
					var vesselTypeNames = string.Join(", ", dock.SupportedVesselTypes.Select(vt => vt.Name));
					
					details = new
					{
						Location = dock.Location,
						Length = dock.PhysicalCharacteristics.Length,
						Depth = dock.PhysicalCharacteristics.Depth,
						Draft = dock.PhysicalCharacteristics.Draft,
						SupportedVesselTypes = vesselTypeNames
					};
				}
				
				chunks.Add(new PortChunk(
					dock.Code,
					ChunkType.Dock,
					baseCol,
					row,
					new { 
						title = dock.Name, 
						description = $"Dock {dock.Code}",
						details = details
					}
				));
				dockIndex++;
			}
		}

			// Add physical resources (STSCranes and YardCranes) at their assigned locations
			foreach (var resource in resources)
			{
				if (resource is STSCraneDto stsCrane)
				{
					// Find the dock this crane serves
					var dockChunk = chunks.FirstOrDefault(c => 
						c.Type == ChunkType.Dock && c.Name == stsCrane.ServingDock.Code);

					if (dockChunk != null)
					{
						object? craneDetails = null;
						if (showDetails)
						{
							var qualNames = string.Join(", ", stsCrane.Qualifications.Select(q => q.QualificationName));
							
							craneDetails = new
							{
								Code = stsCrane.Code,
								Description = stsCrane.Description,
								ServingDock = stsCrane.ServingDock.Name,
								Qualifications = qualNames
							};
						}
						
						// Place crane at same position as its dock
						chunks.Add(new PortChunk(
							stsCrane.Code,
							ChunkType.STSCrane,
							dockChunk.X,
							dockChunk.Y,
							new { 
								title = stsCrane.Code, 
								description = stsCrane.Description,
								details = craneDetails
							}
						));
					}
				}
				else if (resource is YardCraneDto yardCrane)
				{
					// Find yard area for this crane based on qualifications or other criteria
					// For now, place yard cranes at yard storage areas
					var yardChunk = chunks.FirstOrDefault(c => c.Type == ChunkType.Yard);

					if (yardChunk != null)
					{
						object? craneDetails = null;
						if (showDetails)
						{
							var qualNames = string.Join(", ", yardCrane.Qualifications.Select(q => q.QualificationName));
							
							craneDetails = new
							{
								Code = yardCrane.Code,
								Description = yardCrane.Description,
								Qualifications = qualNames
							};
						}
						
						chunks.Add(new PortChunk(
							yardCrane.Code,
							ChunkType.YardCrane,
							yardChunk.X,
							yardChunk.Y,
							new { 
								title = yardCrane.Code, 
								description = yardCrane.Description,
								details = craneDetails
							}
						));
					}
				}
			}

			return Ok(chunks);
		}
		catch (Exception e)
		{
			_logger.LogError("Error generating port layout, {Message}", e.Message);
			return StatusCode(500, "An error occurred while generating the port layout.");
		}
	}
}