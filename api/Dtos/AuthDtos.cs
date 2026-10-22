using System.ComponentModel.DataAnnotations;

namespace Fisgada.Api.Dtos;

public record CadastroRequest(
    [Required, StringLength(80)] string Nome,
    [Required, RegularExpression("^[a-zA-Z0-9_.]{3,30}$",
        ErrorMessage = "Use de 3 a 30 caracteres: letras, números, ponto ou _.")] string NomeUsuario,
    [Required, EmailAddress(ErrorMessage = "E-mail inválido.")] string Email,
    [Required, MinLength(8, ErrorMessage = "A senha precisa ter pelo menos 8 caracteres.")] string Senha);

public record LoginRequest(
    [Required, EmailAddress(ErrorMessage = "E-mail inválido.")] string Email,
    [Required] string Senha);

public record UsuarioResponse(int Id, string Nome, string NomeUsuario, string Email);
public record LoginResponse(string Token, UsuarioResponse Usuario);