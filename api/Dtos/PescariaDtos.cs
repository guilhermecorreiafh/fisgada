using System.ComponentModel.DataAnnotations;
using Fisgada.Api.Models;

namespace Fisgada.Api.Dtos;

public record EspecieResponse(int Id, string NomePopular, string NomeCientifico, string? FotoUrl);
public record PescariaRequest(
    [Range(1, int.MaxValue, ErrorMessage = "Informe a espécie")] int EspecieId,
    DateOnly Data,
    [Required, StringLength(120)] string Local,
    [StringLength(80)] string? Isca,
    [Range(0.1, 9999.9)] decimal? TamanhoCm,
    [Range(0.1, 9999.9)] decimal? PesoKg,
    [Range(1, 5, ErrorMessage = "A nota vai de 1 a 5")] int Nota,
    [StringLength(1000)] string? Comentario,
    bool FoiSolto);

public record PescariaResponse(
    int Id,
    int EspecieId,
    string Especie,
    string Pescador,
    DateOnly Data,
    string Local,
    string? Isca,
    decimal? TamanhoCm,
    decimal? PesoKg,
    int Nota,
    string? Comentario,
    string? FotoUrl,
    bool FoiSolto,
    DateTime CriadoEm);

public static class PescariaMapping
{
    public static PescariaResponse ToResponse(this Pescaria pescaria)
    {
        return new PescariaResponse(
            pescaria.Id,
            pescaria.EspecieId,
            pescaria.Especie.NomePopular,
            pescaria.Usuario.NomeUsuario,
            pescaria.Data,
            pescaria.Local,
            pescaria.Isca,
            pescaria.TamanhoCm,
            pescaria.PesoKg,
            pescaria.Nota,
            pescaria.Comentario,
            pescaria.FotoUrl,
            pescaria.FoiSolto,
            pescaria.CriadoEm);
    }
}