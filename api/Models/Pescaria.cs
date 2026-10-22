namespace Fisgada.Api.Models;

public class Pescaria
{
    public int Id { get; set; }

    public int EspecieId { get; set; }
    public Especie Especie { get; set; } = null!;
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public DateOnly Data { get; set; }
    public string Local { get; set; } = string.Empty;
    public string? Isca { get; set; }
    public decimal? TamanhoCm { get; set; }
    public decimal? PesoKg { get; set; }
    public int Nota { get; set; }
    public string? Comentario { get; set; }
    public string? FotoUrl { get; set; }
    public bool FoiSolto { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}