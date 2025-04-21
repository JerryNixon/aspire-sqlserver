using AlphaDynamics.Poco;
using AlphaDynamics.SqlRepository;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaDynamics.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase
{
    private readonly AlphaDynamicsContext _context;

    public EquipmentController(AlphaDynamicsContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<List<Equipment>>> GetAll() =>
        Ok(await _context.Equipment.ToListAsync());

    [HttpPost]
    public async Task<ActionResult> Create(Equipment equipment)
    {
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = equipment.Id }, equipment);
    }
}
