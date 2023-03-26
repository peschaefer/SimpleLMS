using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulesController : ControllerBase
    {
        private readonly LMSContext _context;

        public ModulesController(LMSContext context)
        {
            _context = context;
        }

        // GET: api/Modules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Module>>> GetModules()
        {
            if (_context.Modules == null)
            {
                return NotFound();
            }
            return await _context.Modules.ToListAsync();
        }

        // GET: api/Modules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Module>> GetModule(int id)
        {
            if (_context.Modules == null)
            {
                return NotFound();
            }
            var @module = await _context.Modules.FindAsync(id);

            if (@module == null)
            {
                return NotFound();
            }

            return @module;
        }

        // GET: api/Modules/5/assignments
        [HttpGet("{id}/Assignments")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignmentsInModule(int id)
        {
            var assignments = await _context.Assignments
                .Where(a => a.ModuleId == id)
                .ToListAsync();

            if (assignments == null)
            {
                return NotFound();
            }

            return assignments;
        }

        // PUT: api/Modules/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModule(int id, Module module)
        {
            if (id != module.Id)
            {
                return BadRequest();
            }

            // see if new course id reference a valid course in the database
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == module.CourseId);
            if (!courseExists)
            {
                return BadRequest("Invalid ModuleId");
            }

            _context.Entry(module).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModuleExists(id))
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

        // POST: api/Modules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Assignment>> PostModule(Module module)
        {
            if (_context.Courses == null)
            {
                return Problem("Entity set 'LMSContext.Modules' is null.");
            }

            var course = await _context.Courses.FindAsync(module.CourseId);

            if (course == null)
            {
                return NotFound("Course not found");
            }

            _context.Modules.Add(module);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetModule", new { id = module.Id }, module);
        }

        // DELETE: api/Modules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
        {
            if (_context.Modules == null)
            {
                return NotFound();
            }
            var @module = await _context.Modules.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }

            _context.Modules.Remove(@module);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModuleExists(int id)
        {
            return (_context.Modules?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
