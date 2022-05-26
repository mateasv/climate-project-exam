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
        private readonly IHubContext<TreeHub,ITreeHubClient> _hubContext;
        private readonly TreeDBContext _context;
        private readonly IMapper _mapper;


        public DataloggerMeasurementsController(
            MeasurementsController measurementsController,
            TreeDBContext context,
            IHubContext<TreeHub,ITreeHubClient> hubContext,
            IMapper mapper
        )
        {
            _measurementsController = measurementsController;
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

        // POST: api/DataloggerMeasurements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MeasurementDto>> PostMeasurement(Measurement measurement)
        {
            var datalogger = await _context.Dataloggers.FindAsync(measurement.DataloggerId);

            if (datalogger == null) return NotFound($"Datalogger not found");

            await _context.Measurements.AddAsync(measurement);
            await _context.SaveChangesAsync();

            var measurementDto = _mapper.Map<MeasurementDto>(measurement);

            if (measurementDto.SoilIsDry || measurementDto.AirTemerature < datalogger.MinAirTemperature)
            {
                await _hubContext.Clients.Group($"Datalogger: {datalogger.DataloggerId}").ReceiveWarning(measurementDto, true);
                await _hubContext.Clients.Group("AppClient").ReceiveWarning(measurementDto, true);
            }
            else
            {
                await _hubContext.Clients.Group($"Datalogger: {datalogger.DataloggerId}").ReceiveWarning(measurementDto, false);
                await _hubContext.Clients.Group("AppClient").ReceiveWarning(measurementDto, false);
            }

            return CreatedAtAction("GetMeasurement", new { id = measurementDto.MeasurementId }, measurementDto);
        }
    }
}
