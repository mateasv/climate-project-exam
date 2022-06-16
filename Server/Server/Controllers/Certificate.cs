using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Hubs;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Certificate : ControllerBase
    {
        private readonly MeasurementsController _measurementsController;
        private readonly DataloggersController _dataloggersController;
        private readonly PlantsController _plantsController;
        private readonly PlantTypesController _plantTypesController;
        private readonly IHubContext<TreeHub, ITreeHubClient> _hubContext;
        private readonly TreeDBContext _context;
        private readonly IMapper _mapper;

        public Certificate(MeasurementsController measurementsController, 
            DataloggersController dataloggersController, 
            IHubContext<TreeHub,
            ITreeHubClient> hubContext, 
            TreeDBContext context, 
            IMapper mapper,
            PlantsController plantsController,
            PlantTypesController plantTypesController)
        {
            _measurementsController = measurementsController;
            _dataloggersController = dataloggersController;
            _hubContext = hubContext;
            _context = context;
            _mapper = mapper;
            _plantsController = plantsController;
            _plantTypesController = plantTypesController;
        }

        // POST: api/Certificate?plantid=1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> CreateCertificate(int plantId)
        {
            // return if invalid plant id
            if(plantId <= 0) return BadRequest();

            // get the measurements for a plant
            var measurementsResult = await _measurementsController.GetMeasurementByPlantId(plantId);

            var measurements = measurementsResult.Value;

            if (measurements is null) return NotFound();


            // get the plant
            var plantResult = await _plantsController.GetPlant(plantId);

            var plant = plantResult.Value;

            if (plant is null) return NotFound();


            // get the plant type
            var plantTypeId = plant?.PlantTypeId;

            if (plantTypeId is null) return NotFound();

            var plantTypeResult = await _plantTypesController.GetPlantType(plantTypeId.GetValueOrDefault());

            var plantType = plantTypeResult.Value;

            if (plantType is null) return NotFound();

           
            // setup data string
            var data = string.Empty;

            // Certificate header
            data += $"Certificate for plant Id: {plantId}\n\n";
            data += $"Plant Type: {plantType.PlantTypeName}\n\n";

            // loop through each measurement and add the data
            foreach (var measurement in measurements)
            {
                
                data += $"Measurement Id: {measurement.MeasurementId} " +
                    $"Datalogger Id: {measurement.DataloggerId} " +
                    $"Plant Id: {measurement.PlantId} " +
                    $"Air Humidity: {measurement.AirHumidity} " +
                    $"Air Temperature: {measurement.AirTemperature} " +
                    $"Soil Is Dry: {measurement.SoilIsDry} " +
                    $"Date: {measurement.MeasurementDate}\n";
            }

            // return if data is null or empty
            if (string.IsNullOrEmpty(data)) return BadRequest();

            // write data to file
            await System.IO.File.WriteAllTextAsync($"certificate-plant-{plant?.PlantId}.txt", data);

            return Ok();
        }
    }
}
