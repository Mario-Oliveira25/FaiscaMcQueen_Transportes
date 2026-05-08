using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
using System.ComponentModel.DataAnnotations;

namespace FaiscaMcQueen_Transportes.ViewModels
{
    public class TecnicoViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O NIF é obrigatório")]
        [RegularExpression(@"^[1235689]\d{8}$", ErrorMessage = "O NIF inválido.")]
        public string Nif { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        public string Especialidade { get; set; } = string.Empty;

        // Adicionamos a lista de Intervenções. 
        // Inicializamos como uma lista vazia para evitar erros caso não haja nenhuma.
        public ICollection<Intervencao> ListaIntervencoes { get; set; } = new List<Intervencao>();
    }
}
