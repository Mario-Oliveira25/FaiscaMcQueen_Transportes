using System.ComponentModel.DataAnnotations;

namespace FaiscaMcQueen_Transportes.Data.FaiscaMcQueen
{
    public class Ativo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "A matrícula é obrigatória.")]
        [RegularExpression(@"^([A-Z]{2}-\d{2}-\d{2}|\d{2}-\d{2}-[A-Z]{2}|\d{2}-[A-Z]{2}-\d{2}|[A-Z]{2}-\d{2}-[A-Z]{2})$",
        ErrorMessage = "A matrícula é inválida. Use um formato português válido (Ex: AA-00-00, 00-AA-00, etc.) e letras maiúsculas.")]
        public string Matricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Tipo de veículo é obrigatório (Ex.: Camião, Máquina)")]
        public string Tipo { get; set; } = string.Empty;

        public string Marca {  get; set; } = string.Empty;

        public string Modelo {  get; set; } = string.Empty;

        public ICollection<Intervencao> Intervencoes { get; set; } = new List<Intervencao>();
    }
}
