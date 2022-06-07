﻿using AutoMapper;
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
            // to do
            var result = await _dataloggersController.GetDatalogger(measurement.DataloggerId.GetValueOrDefault());
            
            if (result.Result is NotFoundResult) return NotFound($"Datalogger not found for id: {measurement.DataloggerId}");
            if (result.Value is null) return NotFound($"Datalogger is null");

            var datalogger = result.Value;

            await _context.Measurements.AddAsync(measurement);
            await _context.SaveChangesAsync();

            var measurementDto = _mapper.Map<MeasurementDto>(measurement);

            if (measurementDto.SoilIsDry || measurementDto.AirTemperature < datalogger.MinAirTemperature)
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
