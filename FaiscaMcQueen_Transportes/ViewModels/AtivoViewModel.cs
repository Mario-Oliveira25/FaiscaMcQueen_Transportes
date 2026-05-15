using System.ComponentModel.DataAnnotations;
namespace FaiscaMcQueen_Transportes.ViewModels
{
    public class AtivoViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "A matrícula é obrigatória.")]
        [RegularExpression ( @"^([A-Z]{2}-\d{2}-\d{2}|\d{2}-\d{2}-[A-Z]{2}|\d{2}-[A-Z]{2}-\d{2}|[A-Z]{2}-\d{2}-[A-Z]{2})$",
            ErrorMessage = "A matrícula é inválida. Use um formato português válido (Ex: AA-00-00, 00-AA-00, etc.) e letras maiúsculas.")]

        public string Matricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Tipo de veículo é obrigatório (Ex.: Camião, Máquina).")]

        public string Tipo { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Modelo { get; set; } = string.Empty;

        public ICollection<IntervencaoDetalhesViewModel> UltimasIntervencoes { get; set; } = new List<IntervencaoDetalhesViewModel>();
    }
    public class IntervencaoDetalhesViewModel
    {
        public Guid Id { get; set; }
        public DateTime DataCriacao { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string TecnicoNome { get; set; } = string.Empty;


        public string DataCriacaoFormatada => DataCriacao.ToString("dd/MM/yyyy HH:mm");

        public string DataInicioFormatada => DataInicio.ToString("dd/MM/yyyy HH:mm");

        public string DataFimFormatada => DataFim?.ToString("dd/MM/yyyy HH:mm") ?? "-";
    }
}
    
