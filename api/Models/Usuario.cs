namespace Fisgada.Api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string NomeUsuario { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public List<Pescaria> Pescarias { get; set; } = [];
}
