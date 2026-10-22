using Fisgada.Api.Data;
using Fisgada.Api.Dtos;
using Fisgada.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Fisgada.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, IPasswordHasher<Usuario> hasher) : ControllerBase
{
    [HttpPost("cadastro")]
    public async Task<ActionResult<UsuarioResponse>> Cadastrar(CadastroRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var nomeUsuario = request.NomeUsuario.Trim().ToLowerInvariant();

        if (await db.Usuarios.AnyAsync(u => u.Email == email))
            ModelState.AddModelError(nameof(request.Email), "Este e-mail já está cadastrado.");

        if (await db.Usuarios.AnyAsync(u => u.NomeUsuario == nomeUsuario))
            ModelState.AddModelError(nameof(request.NomeUsuario), "Este nome de usuário já está em uso.");

        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            NomeUsuario = nomeUsuario,
            Email = email,
        };
        usuario.SenhaHash = hasher.HashPassword(usuario, request.Senha);

        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync();

        var response = new UsuarioResponse(usuario.Id, usuario.Nome, usuario.NomeUsuario, usuario.Email);
        return Created($"/api/usuarios/{usuario.NomeUsuario}", response);
    }
}
