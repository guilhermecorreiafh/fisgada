using System.Security.Claims;
using System.Text;
using Fisgada.Api.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Fisgada.Api.Services;

public class TokenService(IConfiguration config)
{
    public string Gerar(Usuario usuario)
    {
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Chave"]!));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = config["jwt:Emissor"],
            Audience = config["jwt:Publico"],
            Expires = DateTime.UtcNow.AddHours(config.GetValue<int>("Jwt:ExpiracaoHoras")),
            SigningCredentials = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(
                [
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.NomeUsuario),
                ])
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}