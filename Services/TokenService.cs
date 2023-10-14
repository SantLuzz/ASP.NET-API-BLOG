using Blog.Extensions;
using Blog.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blog.Services
{
    public class TokenService
    {
        public string GenerateToken(User user)
        {
            //cria a instancia para gerar o token
            var tokenHandler = new JwtSecurityTokenHandler();
            //pegando a chave e transformando em um array de byte
            var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
            //usando o método de extensão para gerar os clims do usuário
            var claims = user.GetClaims();

            //Configura o token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //coloca o objeto com as informações de chave valor do token
                Subject = new System.Security.Claims.ClaimsIdentity(claims),
                //define o tempo de duração do token
                Expires = DateTime.UtcNow.AddHours(8),
                //define com o token será gerado e lido posteriormente
                SigningCredentials = new SigningCredentials(
                    //passando uma chave simétrica
                    new SymmetricSecurityKey(key),
                    //tipo de algoritimo para encriptar
                    SecurityAlgorithms.HmacSha256Signature),
            };
            //criando o token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //gera uma string baseada no token
            return tokenHandler.WriteToken(token);
        }
    }
}
