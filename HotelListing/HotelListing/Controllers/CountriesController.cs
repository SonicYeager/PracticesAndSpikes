using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Contexts;
using HotelListing.Contracts;
using HotelListing.Entities;
using HotelListing.Models.Country;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;

        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository)
        {
            _countriesRepository = countriesRepository;
            _mapper = mapper;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _countriesRepository.GetAllAsync();
            var countryDtos = _mapper.Map<IEnumerable<GetCountryDto>>(countries);
            
            return Ok(countryDtos);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryDto>> GetCountryEntity(int id)
        {

            var countryEntity = await _countriesRepository.GetDetails(id);

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
            var countryEntity = await _countriesRepository.GetAsync(id);

            if (countryEntity == null)
            {
                return NotFound();
            }
            
            if (id != countryEntity.Id)
            {
                return BadRequest();
            }

            _mapper.Map(updateCountryDto, countryEntity);
            
            try
            {
                await _countriesRepository.UpdateAsync(countryEntity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CountryEntityExists(id))
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

            await _countriesRepository.AddAsync(countryEntity);

            return CreatedAtAction("GetCountryEntity", new
            {
                id = countryEntity.Id
            }, countryEntity);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountryEntity(int id)
        {
            var countryEntity = _countriesRepository.GetAsync(id);
            if (countryEntity == null)
            {
                return NotFound();
            }
            
            await _countriesRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> CountryEntityExists(int id)
        {
            return await _countriesRepository.Exists(id);
        }
    }
}