using System.ComponentModel.DataAnnotations;

namespace FaiscaMcQueen_Transportes.ViewModels
{
    public class NovoTecnicoViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public string Email { get; set; }

        public string Especialidade { get; set; }

        [Required(ErrorMessage = "O NIF é obrigatório")]
        [RegularExpression(@"^[1235689]\d{8}$", ErrorMessage = "O NIF tem de ter 9 números e começar por um dígito válido.")]
        public string Nif { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }
    }
}