using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Contexts;
using HotelListing.Entities;
using HotelListing.Models.Country;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        public CountriesController(HotelListingDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            if (_context.Countries == null)
            {
                return NotFound();
            }
            
            var countries = await _context.Countries.ToListAsync();
            var countryDtos = _mapper.Map<IEnumerable<GetCountryDto>>(countries);
            
            return Ok(countryDtos);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountryEntity(int id)
        {
            if (_context.Countries == null)
            {
                return NotFound();
            }
            
            var countryEntity = await _context.Countries
                .Include(q => q.Hotels)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (countryEntity == null)
            {
                return NotFound();
            }

            var countryDto = _mapper.Map<CountryDto>(countryEntity);

            return Ok(countryDto);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountryEntity(int id, UpdateCountryDto updateCountryDto)
        {

            var countryEntity = await _context.Countries.FindAsync(id);

            if (countryEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(updateCountryDto, countryEntity);
            
            if (id != countryEntity.Id)
            {
                return BadRequest();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryEntityExists(id))
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

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CreateCountryDto>> PostCountryEntity(CreateCountryDto createCountryDto)
        {
            var countryEntity = _mapper.Map<CountryEntity>(createCountryDto);
            
            if (_context.Countries == null)
            {
                return Problem("Entity set 'HotelListingDbContext.Countries'  is null.");
            }
            _context.Countries.Add(countryEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountryEntity", new
            {
                id = countryEntity.Id
            }, countryEntity);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountryEntity(int id)
        {
            if (_context.Countries == null)
            {
                return NotFound();
            }
            var countryEntity = await _context.Countries.FindAsync(id);
            if (countryEntity == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(countryEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CountryEntityExists(int id)
        {
            return (_context.Countries?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}