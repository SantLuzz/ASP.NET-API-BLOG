using Blog.Attributes;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

//Health Check - Checar saude da API
namespace Blog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// //Método que executa um Health Check, onde retorna a saúde da API, 
        /// 200 - funcionando
        /// Outras não está rodando 
        /// Essa função fica na raiz para verificar se está tudo no ar
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        //[ApiKey] //Autenticação por API KEY, para acesso direto na api
        public IActionResult Get(
            [FromServices]EmailService email
        ) 
        {
            return Ok();
        }


    }
}
