using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsServiceController : ControllerBase
    {
        private readonly MeasurementsController _measurementsController;
        private readonly DataloggersController _dataloggersController;
        private readonly PlantsController _plantController;
        private readonly IHubContext<TreeHub> _hubContext;

        public MeasurementsServiceController(
            MeasurementsController measurementsController,
            DataloggersController dataloggersController,
            PlantsController plantsController,
            IHubContext<TreeHub> hubContext
        )
        {
            _measurementsController = measurementsController;
            _dataloggersController = dataloggersController;
            _plantController = plantsController;
            _hubContext = hubContext;
        }

        // GET: api/MeasurementsService
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Measurement>>> GetMeasurements()
        {
            var measurements = await _measurementsController.GetMeasurements();

            return measurements;
        }

        // GET: api/MeasurementsService
        [HttpGet("{id}")]
        public async Task<ActionResult<Measurement>> GetMeasurement(int id)
        {
            var measurements = await _measurementsController.GetMeasurement(id);
            
            if(measurements.Value() == null)
            {

            }

            return measurements;
        }

        // POST: api/MeasurementsService
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Measurement>> PostMeasurement(Measurement measurement)
        {
            var a = _measurementsController.PostMeasurement(measurement);
            
            
            //var datalogger = _dataloggersController.GetDatalogger(measurement.PlantId.GetValueOrDefault());



            return null;
        }
    }
}
