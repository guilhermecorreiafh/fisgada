using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fisgada.Api.Dtos;

namespace Fisgada.Tests;

public class AuthTests(FisgadaFactory factory) : IClassFixture<FisgadaFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Login_ComSenhaCorreta_RetornaToken()
    {
        var token = await FisgadaFactory.CriarUsuarioELogar(_client);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task Login_ComSenhaErrada_Retorna401()
    {
        await _client.PostAsJsonAsync("/api/auth/cadastro",
            new CadastroRequest("Teste", "senhaerrada", "senhaerrada@teste.com", "senha12345"));

        var resposta = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("senhaerrada@teste.com", "outrasenha"));

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Cadastro_ComEmailRepetido_Retorna400()
    {
        var request = new CadastroRequest("Teste", "repetido", "repetido@teste.com", "senha12345");
        await _client.PostAsJsonAsync("/api/auth/cadastro", request);

        var resposta = await _client.PostAsJsonAsync("/api/auth/cadastro", request);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Eu_SemToken_Retorna401()
    {
        var resposta = await _client.GetAsync("/api/auth/eu");

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Eu_ComToken_RetornaUsuarioLogado()
    {
        var token = await FisgadaFactory.CriarUsuarioELogar(_client, "pescador1");
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/auth/eu");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resposta = await _client.SendAsync(request);
        var usuario = await resposta.Content.ReadFromJsonAsync<UsuarioResponse>();

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal("pescador1", usuario!.NomeUsuario);
    }
}