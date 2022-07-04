using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListing.Contracts;
using HotelListing.Entities;
using HotelListing.Models.Configurations;
using HotelListing.Models.Hotel;
using Microsoft.AspNetCore.Authorization;

namespace HotelListing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelsRepository _hotelsRepository;
        private readonly IMapper _mapper;

        public HotelsController(IMapper mapper, IHotelsRepository hotelsRepository)
        {
            _hotelsRepository = hotelsRepository;
            _mapper = mapper;
        }

        // GET: api/Hotels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetHotelDto>>> GetHotels()
        {
            var hotelEntities = await _hotelsRepository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<GetHotelDto>>(hotelEntities));
        }

        // GET: api/Hotels/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<HotelDto>> GetHotelEntity(int id)
        {
            var hotelEntity = await _hotelsRepository.GetAsync(id);

            if (hotelEntity == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<HotelDto>(hotelEntity));
        }

        // PUT: api/Hotels/5
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> PutHotelEntity(int id, UpdateHotelDto updateHotelDto)
        {
            var hotelEntity = await _hotelsRepository.GetAsync(id);

            if (hotelEntity == null)
            {
                return NotFound();
            }

            if (id != hotelEntity.Id)
            {
                return BadRequest();
            }

            _mapper.Map(updateHotelDto, hotelEntity);

            try
            {
                await _hotelsRepository.UpdateAsync(hotelEntity);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await HotelEntityExists(id))
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
        // To protect from over posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<HotelDto>> PostHotelEntity(HotelDto hotelDto)
        {
            var hotelEntity = _mapper.Map<HotelEntity>(hotelDto);
            await _hotelsRepository.AddAsync(hotelEntity);

            return CreatedAtAction("GetHotelEntity", new
            {
                id = hotelEntity.Id
            }, hotelEntity);
        }

        // DELETE: api/Hotels/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = RoleConfiguration.ADMINISTRATOR_ROLE_NAME)]
        public async Task<IActionResult> DeleteHotelEntity(int id)
        {
            var hotelEntity = await _hotelsRepository.GetAsync(id);

            if (hotelEntity == null)
            {
                return NotFound();
            }

            await _hotelsRepository.DeleteAsync(id);

            return NoContent();
        }

        private async Task<bool> HotelEntityExists(int id)
        {
            return await _hotelsRepository.Exists(id);
        }
    }
}