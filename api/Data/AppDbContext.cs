using Fisgada.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Fisgada.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Especie> Especies => Set<Especie>();
    public DbSet<Pescaria> Pescarias => Set<Pescaria>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(u =>
        {
            u.Property(x => x.Nome).HasMaxLength(80).IsRequired();
            u.Property(x => x.NomeUsuario).HasMaxLength(30).IsRequired();
            u.Property(x => x.Email).HasMaxLength(120).IsRequired();
            u.Property(x => x.SenhaHash).IsRequired();
            u.Property(x => x.FotoUrl).HasMaxLength(500);
            u.HasIndex(x => x.NomeUsuario).IsUnique();
            u.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<Especie>(e =>
        {
            e.Property(x => x.NomePopular).HasMaxLength(80).IsRequired();
            e.Property(x => x.NomeCientifico).HasMaxLength(120).IsRequired();
            e.Property(x => x.FotoUrl).HasMaxLength(500);
            e.HasIndex(x => x.NomePopular).IsUnique();
            e.HasData(EspeciesIniciais);
        });

        modelBuilder.Entity<Pescaria>(p =>
        {
            p.Property(x => x.Local).HasMaxLength(120).IsRequired();
            p.Property(x => x.Isca).HasMaxLength(80);
            p.Property(x => x.TamanhoCm).HasPrecision(6, 1);
            p.Property(x => x.PesoKg).HasPrecision(6, 2);
            p.Property(x => x.Comentario).HasMaxLength(1000);
            p.Property(x => x.FotoUrl).HasMaxLength(500);
            p.ToTable(t => t.HasCheckConstraint("CK_Pescaria_Nota", "[Nota] BETWEEN 1 AND 5"));

            p.HasOne(x => x.Especie)
             .WithMany(e => e.Pescarias)
             .HasForeignKey(x => x.EspecieId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static readonly Especie[] EspeciesIniciais =
    [
        new() { Id = 1,  NomePopular = "Robalo-flecha",   NomeCientifico = "Centropomus undecimalis" },
        new() { Id = 2,  NomePopular = "Robalo-peva",     NomeCientifico = "Centropomus parallelus" },
        new() { Id = 3,  NomePopular = "Tainha",          NomeCientifico = "Mugil liza" },
        new() { Id = 4,  NomePopular = "Parati",          NomeCientifico = "Mugil curema" },
        new() { Id = 5,  NomePopular = "Corvina",         NomeCientifico = "Micropogonias furnieri" },
        new() { Id = 6,  NomePopular = "Pescada-amarela", NomeCientifico = "Cynoscion acoupa" },
        new() { Id = 7,  NomePopular = "Pescada-branca",  NomeCientifico = "Cynoscion leiarchus" },
        new() { Id = 8,  NomePopular = "Betara",          NomeCientifico = "Menticirrhus americanus" },
        new() { Id = 9,  NomePopular = "Bagre-branco",    NomeCientifico = "Genidens barbus" },
        new() { Id = 10, NomePopular = "Xaréu",           NomeCientifico = "Caranx hippos" },
        new() { Id = 11, NomePopular = "Carapau",         NomeCientifico = "Caranx crysos" },
        new() { Id = 12, NomePopular = "Sororoca",        NomeCientifico = "Scomberomorus brasiliensis" },
        new() { Id = 13, NomePopular = "Espada",          NomeCientifico = "Trichiurus lepturus" },
        new() { Id = 14, NomePopular = "Anchova",         NomeCientifico = "Pomatomus saltatrix" },
        new() { Id = 15, NomePopular = "Pampo",           NomeCientifico = "Trachinotus carolinus" },
        new() { Id = 16, NomePopular = "Caratinga",       NomeCientifico = "Eugerres brasilianus" },
        new() { Id = 17, NomePopular = "Carapeba",        NomeCientifico = "Diapterus rhombeus" },
        new() { Id = 18, NomePopular = "Linguado",        NomeCientifico = "Paralichthys orbignyanus" },
        new() { Id = 19, NomePopular = "Garoupa",         NomeCientifico = "Epinephelus marginatus" },
        new() { Id = 20, NomePopular = "Baiacu",          NomeCientifico = "Sphoeroides testudineus" },
    ];
}