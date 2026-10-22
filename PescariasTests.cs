using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fisgada.Api.Dtos;

namespace Fisgada.Tests;

public class PescariasTests(FisgadaFactory factory) : IClassFixture<FisgadaFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static PescariaRequest RobaloValido(int nota = 5) => new(
        EspecieId: 1,
        Data: new DateOnly(2026, 10, 18),
        Local: "Píer da Ponta da Praia",
        Isca: "Camarão vivo",
        TamanhoCm: 52.5m,
        PesoKg: 1.8m,
        Nota: nota,
        Comentario: "Briga boa.",
        FoiSolto: true);

    private async Task<HttpResponseMessage> Enviar(HttpMethod metodo, string url, string? token, object? body = null)
    {
        var request = new HttpRequestMessage(metodo, url);
        if (token is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body is not null)
            request.Content = JsonContent.Create(body);

        return await _client.SendAsync(request);
    }

    [Fact]
    public async Task Criar_SemToken_Retorna401()
    {
        var resposta = await Enviar(HttpMethod.Post, "/api/pescarias", null, RobaloValido());

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Criar_ComToken_PreencheDonoPeloToken()
    {
        var token = await FisgadaFactory.CriarUsuarioELogar(_client, "dono1");

        var resposta = await Enviar(HttpMethod.Post, "/api/pescarias", token, RobaloValido());
        var pescaria = await resposta.Content.ReadFromJsonAsync<PescariaResponse>();

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.Equal("dono1", pescaria!.Pescador);
    }

    [Fact]
    public async Task Criar_ComNotaInvalida_Retorna400()
    {
        var token = await FisgadaFactory.CriarUsuarioELogar(_client);

        var resposta = await Enviar(HttpMethod.Post, "/api/pescarias", token, RobaloValido(nota: 7));

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Excluir_PescariaDeOutroUsuario_Retorna403()
    {
        var tokenDono = await FisgadaFactory.CriarUsuarioELogar(_client);
        var tokenOutro = await FisgadaFactory.CriarUsuarioELogar(_client);

        var criada = await Enviar(HttpMethod.Post, "/api/pescarias", tokenDono, RobaloValido());
        var pescaria = await criada.Content.ReadFromJsonAsync<PescariaResponse>();

        var resposta = await Enviar(HttpMethod.Delete, $"/api/pescarias/{pescaria!.Id}", tokenOutro);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task Excluir_PropriaPescaria_Retorna204()
    {
        var token = await FisgadaFactory.CriarUsuarioELogar(_client);

        var criada = await Enviar(HttpMethod.Post, "/api/pescarias", token, RobaloValido());
        var pescaria = await criada.Content.ReadFromJsonAsync<PescariaResponse>();

        var resposta = await Enviar(HttpMethod.Delete, $"/api/pescarias/{pescaria!.Id}", token);

        Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
    }
}