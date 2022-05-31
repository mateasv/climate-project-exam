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
        private readonly DataloggersController _dataloggersController;

        public DataloggerPairController(
            IHubContext<TreeHub,
            ITreeHubClient> hubContext, 
            TreeDBContext context, 
            IMapper mapper, 
            DataloggersController dataloggersController
        )
        {
            _hubContext = hubContext;
            _context = context;
            _mapper = mapper;
            _dataloggersController = dataloggersController;
        }



        // PUT: api/DataloggerPair/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDataloggerPair(int id, Datalogger datalogger)
        {
            var result = await _dataloggersController.PutDatalogger(id, datalogger);

            if(result is NoContentResult)
            {
                var dataloggerDto = _mapper.Map<DataloggerDto>(datalogger);

                await _hubContext.Clients.Group($"Datalogger: {datalogger.DataloggerId}").ReceiveDataloggerPair(dataloggerDto);
            }

            return result;
        }
    }
}
