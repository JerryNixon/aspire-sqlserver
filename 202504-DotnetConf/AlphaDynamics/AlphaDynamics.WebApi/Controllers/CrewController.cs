using AlphaDynamics.Poco;
using AlphaDynamics.SqlRepository;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaDynamics.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CrewController : ControllerBase
{
    private readonly AlphaDynamicsContext _context;

    public CrewController(AlphaDynamicsContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Crew>>> GetAll()
    {
        var crew = await _context.Crews.ToListAsync();
        return Ok(crew);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Crew crew)
    {
        _context.Crews.Add(crew);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = crew.Id }, crew);
    }
}