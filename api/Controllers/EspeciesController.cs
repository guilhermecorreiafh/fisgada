using Fisgada.Api.Data;
using Fisgada.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fisgada.Api.Controllers;

[ApiController]
[Route("api/especies")]
public class EspeciesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<EspecieResponse>> Listar() =>
        await db.Especies
            .OrderBy(e => e.NomePopular)
            .Select(e => new EspecieResponse(e.Id, e.NomePopular, e.NomeCientifico, e.FotoUrl))
            .ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EspecieResponse>> Obter(int id)
    {
        var especie = await db.Especies.FindAsync(id);
        if (especie is null) return NotFound();

        return new EspecieResponse(especie.Id, especie.NomePopular, especie.NomeCientifico, especie.FotoUrl);
    }
}