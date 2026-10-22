using Fisgada.Api.Data;
using Fisgada.Api.Dtos;
using Fisgada.Api.Models;
using Fisgada.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Fisgada.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, IPasswordHasher<Usuario> hasher, TokenService tokens) : ControllerBase
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

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Logar(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario is null || hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, request.Senha) == PasswordVerificationResult.Failed)
        {
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "E-mail ou senha inválidos.");
        }
        
        return new LoginResponse(tokens.Gerar(usuario), ParaResponse(usuario));
    }

    [Authorize]
    [HttpGet("eu")]
    public async Task<ActionResult<UsuarioResponse>> Eu()
    {
        var id = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);
        var usuario = await db.Usuarios.FindAsync(id);
        if (usuario is null) return Unauthorized();

        return ParaResponse(usuario);
    }

    private static UsuarioResponse ParaResponse(Usuario usuario) =>
        new(usuario.Id, usuario.Nome, usuario.NomeUsuario, usuario.Email);
}
