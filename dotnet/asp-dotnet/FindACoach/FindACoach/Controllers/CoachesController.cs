using FindACoach.Persistence;
using FindACoach.Records;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FindACoach.Controllers;

[ApiController]
[Route("[controller]")]
public class CoachesController : ControllerBase
{
    private readonly FindACoachDbContext _dbContext;

    public CoachesController(FindACoachDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("GetCoaches")]
    public async Task<IEnumerable<Coach>> Get()
    {
        return await _dbContext.Set<Coach>().ToListAsync();
    }

    [HttpPut("SetCoach")]
    public async Task<Coach> Put(Coach coach)
    {
        var exists = await _dbContext.Set<Coach>().AnyAsync(c => c.Id == coach.Id);
        if (exists)
        {
            _dbContext.Set<Coach>().Update(coach);
        }
        else
        {
            _dbContext.Set<Coach>().Add(coach);
        }

        await _dbContext.SaveChangesAsync();
        return coach;
    }
}