using Fisgada.Api.Data;
using Fisgada.Api.Dtos;
using Fisgada.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Fisgada.Api.Controllers;

[ApiController]
[Route("api/pescarias")]
public class PescariasController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IEnumerable<PescariaResponse>> Recentes() =>
        (await db.Pescarias
            .Include(p => p.Especie)
            .Include(p => p.Usuario)
            .OrderByDescending(p => p.Data)
            .ThenByDescending(p => p.CriadoEm)
            .Take(50)
            .ToListAsync())
        .Select(p => p.ToResponse());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PescariaResponse>> Obter(int id)
    {
        var pescaria = await db.Pescarias
            .Include(p => p.Especie)
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (pescaria is null) return NotFound();
        return pescaria.ToResponse();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PescariaResponse>> Criar(PescariaRequest request)
    {
        var erro = await Validar(request);
        if (erro is not null) return erro;

        var pescaria = new Pescaria { UsuarioId = UsuarioLogadoId() };
        Aplicar(pescaria, request);

        db.Pescarias.Add(pescaria);
        await db.SaveChangesAsync();
        await CarregarRelacionados(pescaria);

        return CreatedAtAction(nameof(Obter), new { id = pescaria.Id }, pescaria.ToResponse());
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<PescariaResponse>> Atualizar(int id, PescariaRequest request)
    {
        var pescaria = await db.Pescarias.FindAsync(id);
        if (pescaria is null) return NotFound();
        if (pescaria.UsuarioId != UsuarioLogadoId()) return Forbid();

        var erro = await Validar(request);
        if (erro is not null) return erro;

        Aplicar(pescaria, request);
        await db.SaveChangesAsync();
        await CarregarRelacionados(pescaria);

        return pescaria.ToResponse();
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Excluir(int id)
    {
        var pescaria = await db.Pescarias.FindAsync(id);
        if (pescaria is null) return NotFound();
        if (pescaria.UsuarioId != UsuarioLogadoId()) return Forbid();

        db.Pescarias.Remove(pescaria);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private int UsuarioLogadoId() =>
        int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

    private async Task CarregarRelacionados(Pescaria pescaria)
    {
        await db.Entry(pescaria).Reference(p => p.Especie).LoadAsync();
        await db.Entry(pescaria).Reference(p => p.Usuario).LoadAsync();
    }

    private async Task<ActionResult?> Validar(PescariaRequest request)
    {
        if (request.Data > DateOnly.FromDateTime(DateTime.Now))
            ModelState.AddModelError(nameof(request.Data), "A data da pescaria não pode ser no futuro.");

        if (!await db.Especies.AnyAsync(e => e.Id == request.EspecieId))
            ModelState.AddModelError(nameof(request.EspecieId), "A espécie informada não existe.");

        return ModelState.IsValid ? null : ValidationProblem(ModelState);
    }

    private static void Aplicar(Pescaria pescaria, PescariaRequest request)
    {
        pescaria.EspecieId = request.EspecieId;
        pescaria.Data = request.Data;
        pescaria.Local = request.Local.Trim();
        pescaria.Isca = request.Isca?.Trim();
        pescaria.TamanhoCm = request.TamanhoCm;
        pescaria.PesoKg = request.PesoKg;
        pescaria.Nota = request.Nota;
        pescaria.Comentario = request.Comentario?.Trim();
        pescaria.FoiSolto = request.FoiSolto;
    }
}