using AlphaDynamics.Poco;
using AlphaDynamics.SqlRepository;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AlphaDynamics.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly AlphaDynamicsContext _context;

    public TransactionController(AlphaDynamicsContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<List<Transaction>>> GetAll() =>
        Ok(await _context.Transactions
            .Include(t => t.Crew)
            .Include(t => t.Equipment)
            .Include(t => t.Operation)
            .ToListAsync());

    [HttpPost]
    public async Task<ActionResult> Create(Transaction transaction)
    {
        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = transaction.Id }, transaction);
    }
}
