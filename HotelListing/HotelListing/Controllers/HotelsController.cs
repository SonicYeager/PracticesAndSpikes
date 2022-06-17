using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Contexts;
using HotelListing.Entities;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly HotelListingDbContext _context;

        public HotelsController(HotelListingDbContext context)
        {
            _context = context;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelEntity>>> GetHotels()
        {
            if (_context.Hotels == null)
            {
                return NotFound();
            }
            return await _context.Hotels.ToListAsync();
        }

        // GET: api/Hotels/5
        [HttpGet("{id}")]
        public async Task<ActionResult<HotelEntity>> GetHotelEntity(int id)
        {
            if (_context.Hotels == null)
            {
                return NotFound();
            }
            var hotelEntity = await _context.Hotels.FindAsync(id);

            if (hotelEntity == null)
            {
                return NotFound();
            }

            return hotelEntity;
        }

        // PUT: api/Hotels/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHotelEntity(int id, HotelEntity hotelEntity)
        {
            if (id != hotelEntity.Id)
            {
                return BadRequest();
            }

            _context.Entry(hotelEntity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HotelEntityExists(id))
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

        // POST: api/Hotels
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<HotelEntity>> PostHotelEntity(HotelEntity hotelEntity)
        {
            if (_context.Hotels == null)
            {
                return Problem("Entity set 'HotelListingDbContext.Hotels'  is null.");
            }
            _context.Hotels.Add(hotelEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHotelEntity", new
            {
                id = hotelEntity.Id
            }, hotelEntity);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelEntity(int id)
        {
            if (_context.Hotels == null)
            {
                return NotFound();
            }
            var hotelEntity = await _context.Hotels.FindAsync(id);
            if (hotelEntity == null)
            {
                return NotFound();
            }

            _context.Hotels.Remove(hotelEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HotelEntityExists(int id)
        {
            return (_context.Hotels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}