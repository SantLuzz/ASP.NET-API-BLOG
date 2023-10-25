using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nome obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-mail Obrigatório")]
        [EmailAddress(ErrorMessage = "O E-mail é Inválido!")]
        public string Email { get; set; }

    }
}
