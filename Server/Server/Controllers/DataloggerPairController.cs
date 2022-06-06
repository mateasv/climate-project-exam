using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Dtos;
using Server.Hubs;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataloggerPairController : ControllerBase
    {
        private readonly IHubContext<TreeHub, ITreeHubClient> _hubContext;
        private readonly TreeDBContext _context;
        private readonly IMapper _mapper;
        private readonly PlantsController _plantsController;

        public DataloggerPairController(
            IHubContext<TreeHub,
            ITreeHubClient> hubContext, 
            TreeDBContext context, 
            IMapper mapper,
            PlantsController plantsController
        )
        {
            _hubContext = hubContext;
            _context = context;
            _mapper = mapper;
            _plantsController = plantsController;
        }

        // GET: api/Plants/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Plant>> GetPlant(int id)
        {
            if (_context.Plants == null)
            {
                return NotFound();
            }
            var plant = await _context.Plants.FindAsync(id);

            if (plant == null)
            {
                return NotFound();
            }

            return plant;
        }

        // POST: api/DataloggerPair
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Plant>> PostPair(Plant plant)
        {
            var result = await _plantsController.PostPlant(plant);

            if (result is not null)
            {
                var plantDto = _mapper.Map<PlantDto>(plant);

                await _hubContext.Clients.Group($"Datalogger: {plant.DataloggerId}").ReceiveDataloggerPair(plantDto);
            }

            return CreatedAtAction("GetPlant", new { id = plant.PlantId }, plant);
        }

        // PUT: api/dataloggerpair/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPair(int id, Plant plant)
        {
            var result = await _plantsController.PutPlant(id, plant);

            if (result is NoContentResult)
            {
                var plantDto = _mapper.Map<PlantDto>(plant);

                await _hubContext.Clients.Group($"Datalogger: {plantDto.DataloggerId}").ReceiveDataloggerPair(plantDto);
            }

            return result;
        }

        // PUT: api/DataloggerPair/removepair/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("removepair/{id}")]
        public async Task<IActionResult> RemovePair(int id, Plant plant)
        {
            var oldDataloggerId = plant.DataloggerId;

            plant.DataloggerId = null;

            var unpairResult = await _plantsController.PutPlant(id, plant);

            if (unpairResult is NoContentResult)
            {
                var plantDto = _mapper.Map<PlantDto>(plant);

                await _hubContext.Clients.Group($"Datalogger: {oldDataloggerId}").ReceiveDataloggerPair(plantDto);
            }

            return unpairResult;
        }
    }
}
