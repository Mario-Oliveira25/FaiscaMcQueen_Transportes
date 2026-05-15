using System.ComponentModel.DataAnnotations;

namespace FaiscaMcQueen_Transportes.ViewModels
{
    public class DefinirPasswordViewModel
    {
        // Estes dois campos vão estar escondidos (hidden) na View. 
        // O utilizador não mexe neles, mas precisamos deles para saber QUEM é e se a CHAVE é válida.
        public string Email { get; set; }
        public string Token { get; set; }

        [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Tem de confirmar a palavra-passe.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "As palavras-passe não são iguais.")]
        public string ConfirmarPassword { get; set; }
    }
}
