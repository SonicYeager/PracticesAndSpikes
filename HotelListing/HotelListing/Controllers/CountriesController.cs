using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data.Entities;
using HotelListing.Api.Data.Models;
using HotelListing.Api.Data.Models.Configurations;
using HotelListing.Api.Data.Models.Country;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;

namespace HotelListing.Controllers
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CountriesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICountriesRepository _countriesRepository;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(IMapper mapper, ICountriesRepository countriesRepository, ILogger<CountriesController> logger)
        {
            _countriesRepository = countriesRepository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: api/Countries/GetAll
        [HttpGet("GetAll")]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<GetCountryDto>>> GetCountries()
        {
            var countries = await _countriesRepository.GetAllAsync();
            var countryDtos = _mapper.Map<IEnumerable<GetCountryDto>>(countries);

            return Ok(countryDtos);
        }

        // GET: api/Countries/?StartIndex=0&pageSize=25&PageNumber=1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QueryResult<GetCountryDto>>>> GetPagedCountries(
            [FromQuery] QueryParameters queryParameters)
        {
            var countries = await _countriesRepository.GetAllAsync<GetCountryDto>(queryParameters);

            return Ok(countries);
        }

        // GET: api/Countries/5
        [HttpGet("{id:int}")]
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
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> PutCountryEntity(int id, UpdateCountryDto updateCountryDto)
        {
            var countryEntity = await _countriesRepository.GetAsync(id);

            if (countryEntity == null)
            {
                _logger.LogWarning($"Could not find Country with id {id}.");
                return NotFound();
            }

            if (id != countryEntity.Id)
            {
                _logger.LogWarning($"Can not update Country with id {updateCountryDto.Id}. for Id {id}.");
                return BadRequest();
            }

            _mapper.Map(updateCountryDto, countryEntity);

            await _countriesRepository.UpdateAsync(countryEntity);

            return NoContent();
        }

        // POST: api/Countries
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
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
        [HttpDelete("{id:int}")]
        [Authorize(Roles = RoleConfiguration.ADMINISTRATOR_ROLE_NAME)]
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
    }
}