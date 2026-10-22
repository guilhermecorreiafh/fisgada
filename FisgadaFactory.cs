using System.Net.Http.Json;
using Fisgada.Api.Data;
using Fisgada.Api.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Fisgada.Tests;

public class FisgadaFactory : WebApplicationFactory<Program>
{
    private readonly string _nomeBanco = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Chave"] = "chave-de-teste-com-mais-de-32-caracteres-1234567890",
                ["Jwt:Emissor"] = "fisgada-api",
                ["Jwt:Publico"] = "fisgada-web",
                ["Jwt:ExpiracaoHoras"] = "1"
            }));

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(_nomeBanco));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();

        return host;
    }

    public static async Task<string> CriarUsuarioELogar(HttpClient client, string? nomeUsuario = null)
    {
        nomeUsuario ??= "u" + Guid.NewGuid().ToString("N")[..10];
        var email = $"{nomeUsuario}@teste.com";

        await client.PostAsJsonAsync("/api/auth/cadastro",
            new CadastroRequest("Usuário Teste", nomeUsuario, email, "senha12345"));

        var resposta = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "senha12345"));
        var login = await resposta.Content.ReadFromJsonAsync<LoginResponse>();
        return login!.Token;
    }
}