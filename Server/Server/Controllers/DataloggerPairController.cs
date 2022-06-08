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

        /// <summary>
        /// Inserts a new plant with pair into the database and informs the corresponding datalogger
        /// a new pairing has happened.
        /// </summary>
        /// <param name="plant"></param>
        /// <returns></returns>
        // POST: api/DataloggerPair
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Plant>> PostPair(Plant plant)
        {
            // Post the plant into the database
            var result = await _plantsController.PostPlant(plant);


            // if the posting is successfull
            if (result is not null)
            {
                // Map the plant to a dto
                var plantDto = _mapper.Map<PlantDto>(plant);

                // Inform the paired datalogger, that a new pairing has been performed
                await _hubContext.Clients.Group($"Datalogger: {plant.DataloggerId}").ReceiveDataloggerPair(plantDto);
            }

            return CreatedAtAction("GetPlant", new { id = plant.PlantId }, plant);
        }

        /// <summary>
        /// Updates a plant with a new pair
        /// </summary>
        /// <param name="id"></param>
        /// <param name="plant"></param>
        /// <returns></returns>
        // PUT: api/dataloggerpair/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPair(int id, Plant plant)
        {
            // Modifies the plant and informs the corresponding datalogger that a new pairing has happened
            var result = await _plantsController.PutPlant(id, plant);

            if (result is NoContentResult)
            {
                // map plant to a dto
                var plantDto = _mapper.Map<PlantDto>(plant);

                // call the datalogger
                await _hubContext.Clients.Group($"Datalogger: {plantDto.DataloggerId}").ReceiveDataloggerPair(plantDto);
            }

            return result;
        }

        /// <summary>
        /// Removes an existing pair for a plant
        /// </summary>
        /// <param name="id"></param>
        /// <param name="plant"></param>
        /// <returns></returns>
        // PUT: api/DataloggerPair/removepair/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("removepair/{id}")]
        public async Task<IActionResult> RemovePair(int id, Plant plant)
        {
            // Save the old datalogger id for the plant
            var oldDataloggerId = plant.DataloggerId;

            // set the plants datalogger id to null
            plant.DataloggerId = null;

            // modify the plant and save changes in the database
            var unpairResult = await _plantsController.PutPlant(id, plant);

            // if unpairing is successfull
            if (unpairResult is NoContentResult)
            {
                // map plant to a dto
                var plantDto = _mapper.Map<PlantDto>(plant);

                // inform the overwritten pairs datalogger that it has been unpaired
                await _hubContext.Clients.Group($"Datalogger: {oldDataloggerId}").ReceiveDataloggerPair(plantDto);
            }

            return unpairResult;
        }
    }
}
