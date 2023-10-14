using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.Extensions
{
    public static class ModelStateExtension
    {
        /// <summary>
        /// Método de extensão para o model state pegando todos os erros e convertendo para uma lista de string,
        /// neste caso para acessar é usando ModelState.GetErros() dentro de qualquer instancia do mesmo
        /// </summary>
        /// <param name="modelState">Recebe o modelState instanciado</param>
        /// <returns>Uma coleção de string com os erros do método</returns>
        public static List<string> GetErrors(this ModelStateDictionary modelState)
        {
            var result = new List<string>();
            //carregando os erros
            foreach (var item in modelState.Values)
                result.AddRange(item.Errors.Select(error => error.ErrorMessage));
            
            return result;
        }
    }
}
