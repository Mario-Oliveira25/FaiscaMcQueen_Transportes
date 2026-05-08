using System.ComponentModel.DataAnnotations;

namespace FaiscaMcQueen_Transportes.Data.FaiscaMcQueen
{
    public class Tecnico
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O NIF é obrigatório")]
        [RegularExpression(@"^[1235689]\d{8}$", ErrorMessage = "O NIF tem de ter 9 números e começar por um dígito válido.")]
        public string Nif { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        public string Especialidade { get; set; } = string.Empty;

        public ICollection<Intervencao> Intervencoes { get; set; } = new List<Intervencao>();
    }
}
