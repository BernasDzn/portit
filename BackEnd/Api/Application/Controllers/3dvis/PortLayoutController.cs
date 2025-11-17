using Microsoft.AspNetCore.Mvc;
using Api.Application.Services;
using Api.Application.DataTransfer;

namespace Api.Application.Controllers;

[ApiController]
[Route("[controller]")]
public class PortLayoutController : ControllerBase
{
	private readonly IDockService _dockService;
	private readonly IStorageAreaService _storageAreaService;
	private readonly IPhysicalResourceService _physicalResourceService;
	private readonly ILogger<PortLayoutController> _logger;

	public PortLayoutController(
		IDockService dockService,
		IStorageAreaService storageAreaService,
		IPhysicalResourceService physicalResourceService,
		ILogger<PortLayoutController> logger)
	{
		_dockService = dockService;
		_storageAreaService = storageAreaService;
		_physicalResourceService = physicalResourceService;
		_logger = logger;
	}

	[HttpGet(Name = "GetPortLayout")]
	public async Task<ActionResult<IEnumerable<PortChunk>>> GetPortLayout()
	{
		try
		{
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
						var chunkType = storage.Type == Domain.Entities.StorageAreaType.Warehouse 
							? ChunkType.Warehouse 
							: ChunkType.Yard;

						chunks.Add(new PortChunk(
							storage.NameCode,
							chunkType,
							col,
							row
						));
						storageIndex++;
					}else{
						// Fill remaining spaces with Land chunks
						chunks.Add(new PortChunk(
							"Land",
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
					chunks.Add(new PortChunk(
						dock.Code,
						ChunkType.Dock,
						baseCol,
						row
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
						// Place crane at same position as its dock
						chunks.Add(new PortChunk(
							stsCrane.Code,
							ChunkType.STSCrane,
							dockChunk.X,
							dockChunk.Y
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
						chunks.Add(new PortChunk(
							yardCrane.Code,
							ChunkType.YardCrane,
							yardChunk.X,
							yardChunk.Y
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