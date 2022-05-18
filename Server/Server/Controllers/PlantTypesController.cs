using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Server.Hubs;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantTypesController : ControllerBase
    {
        private readonly TreeDBContext _context;
        private readonly IHubContext<TreeHub> _hubContext;

        public PlantTypesController(TreeDBContext context, IHubContext<TreeHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/PlantTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlantType>>> GetPlantTypes()
        {
          if (_context.PlantTypes == null)
          {
              return NotFound();
          }
            return await _context.PlantTypes.ToListAsync();
        }

        // GET: api/PlantTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlantType>> GetPlantType(int id)
        {
          if (_context.PlantTypes == null)
          {
              return NotFound();
          }
            var plantType = await _context.PlantTypes.FindAsync(id);

            if (plantType == null)
            {
                return NotFound();
            }

            return plantType;
        }

        // PUT: api/PlantTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlantType(int id, PlantType plantType)
        {
            if (id != plantType.PlantTypeId)
            {
                return BadRequest();
            }

            _context.Entry(plantType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PlantTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlantType>> PostPlantType(PlantType plantType)
        {
          if (_context.PlantTypes == null)
          {
              return Problem("Entity set 'TreeDBContext.PlantTypes'  is null.");
          }
            _context.PlantTypes.Add(plantType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlantType", new { id = plantType.PlantTypeId }, plantType);
        }

        // DELETE: api/PlantTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlantType(int id)
        {
            if (_context.PlantTypes == null)
            {
                return NotFound();
            }
            var plantType = await _context.PlantTypes.FindAsync(id);
            if (plantType == null)
            {
                return NotFound();
            }

            _context.PlantTypes.Remove(plantType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlantTypeExists(int id)
        {
            return (_context.PlantTypes?.Any(e => e.PlantTypeId == id)).GetValueOrDefault();
        }
    }
}
