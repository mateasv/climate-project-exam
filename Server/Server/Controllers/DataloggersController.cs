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
    public class DataloggersController : ControllerBase
    {
        private readonly TreeDBContext _context;
        private readonly IHubContext<TreeHub> _hubContext;

        public DataloggersController(TreeDBContext context, IHubContext<TreeHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // GET: api/Dataloggers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Datalogger>>> GetDataloggers()
        {
          if (_context.Dataloggers == null)
          {
              return NotFound();
          }
            return await _context.Dataloggers.ToListAsync();
        }

        // GET: api/Dataloggers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Datalogger>> GetDatalogger(int id)
        {
          if (_context.Dataloggers == null)
          {
              return NotFound();
          }
            var datalogger = await _context.Dataloggers.FindAsync(id);

            if (datalogger == null)
            {
                return NotFound();
            }

            return datalogger;
        }

        // PUT: api/Dataloggers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDatalogger(int id, Datalogger datalogger)
        {
            if (id != datalogger.DataloggerId)
            {
                return BadRequest();
            }

            _context.Entry(datalogger).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataloggerExists(id))
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

        // POST: api/Dataloggers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Datalogger>> PostDatalogger(Datalogger datalogger)
        {
          if (_context.Dataloggers == null)
          {
              return Problem("Entity set 'TreeDBContext.Dataloggers'  is null.");
          }
            _context.Dataloggers.Add(datalogger);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDatalogger", new { id = datalogger.DataloggerId }, datalogger);
        }

        // DELETE: api/Dataloggers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDatalogger(int id)
        {
            if (_context.Dataloggers == null)
            {
                return NotFound();
            }
            var datalogger = await _context.Dataloggers.FindAsync(id);
            if (datalogger == null)
            {
                return NotFound();
            }

            _context.Dataloggers.Remove(datalogger);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DataloggerExists(int id)
        {
            return (_context.Dataloggers?.Any(e => e.DataloggerId == id)).GetValueOrDefault();
        }
    }
}
