namespace Fisgada.Api.Models;

public class Especie
{
    public int Id { get; set; }
    public string NomePopular { get; set; } = string.Empty;
    public string NomeCientifico { get; set; } = string.Empty;
    public string? FotoUrl { get; set; }

    public List<Pescaria> Pescarias { get; set; } = [];
}