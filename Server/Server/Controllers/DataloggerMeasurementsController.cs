using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Dtos;
using Server.Hubs;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataloggerMeasurementsController : ControllerBase
    {
        private readonly MeasurementsController _measurementsController;
        private readonly DataloggersController _dataloggersController;
        private readonly IHubContext<TreeHub,ITreeHubClient> _hubContext;
        private readonly TreeDBContext _context;
        private readonly IMapper _mapper;


        public DataloggerMeasurementsController(MeasurementsController measurementsController, 
            DataloggersController dataloggersController, 
            IHubContext<TreeHub, 
            ITreeHubClient> hubContext,
            TreeDBContext context,IMapper mapper
        )
        {
            _measurementsController = measurementsController;
            _dataloggersController = dataloggersController;
            _hubContext = hubContext;
            _context = context;
            _mapper = mapper;
        }

        // GET: api/DataloggerMeasurements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Measurement>> GetMeasurement(int id)
        {
            return await _measurementsController.GetMeasurement(id);
        }


        /// <summary>
        /// Endpoint for the dataloggers. This endpoint inserts a measurement into the database, and if
        /// successful, call the ReceiveWarning on the SignalR clients
        /// </summary>
        /// <param name="measurement"></param>
        /// <returns></returns>
        // POST: api/DataloggerMeasurements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MeasurementDto>> PostMeasurement(Measurement measurement)
        {
            // Find the datalogger which performed the measurement
            var result = await _dataloggersController.GetDatalogger(measurement.DataloggerId.GetValueOrDefault());
            
            // Perform checks if the datalogger is found
            if (result.Result is NotFoundResult) return NotFound($"Datalogger not found for id: {measurement.DataloggerId}");
            if (result.Value is null) return NotFound($"Datalogger is null");

            // Get the datalogger object
            var datalogger = result.Value;

            // Insert the measurement into the database
            await _context.Measurements.AddAsync(measurement);
            await _context.SaveChangesAsync();

            // Prepare the measurement object by mapping it to a data transfer object (DTO)
            var measurementDto = _mapper.Map<MeasurementDto>(measurement);

            // If the measurement evaluates to dry or if the measured air temperature is less than the datalogger air temperature limit
            if (measurementDto.SoilIsDry || measurementDto.AirTemperature < datalogger.MinAirTemperature)
            {
                // Contact the datalogger that performed the measurement about the warning
                await _hubContext.Clients.Group($"Datalogger: {datalogger.DataloggerId}").ReceiveWarning(measurementDto, true);

                // Contact the mobile clients, and inform them about the warning
                await _hubContext.Clients.Group("AppClient").ReceiveWarning(measurementDto, true);
            }
            else
            {
                // Contact the datalogger that the performed measurement is not a warning
                await _hubContext.Clients.Group($"Datalogger: {datalogger.DataloggerId}").ReceiveWarning(measurementDto, false);

                // Contact the mobile clients, and inform them that the measurement is not a warning
                await _hubContext.Clients.Group("AppClient").ReceiveWarning(measurementDto, false);
            }

            return CreatedAtAction("GetMeasurement", new { id = measurementDto.MeasurementId }, measurementDto);
        }
    }
}
