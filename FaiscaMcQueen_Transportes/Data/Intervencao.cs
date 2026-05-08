using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;


namespace FaiscaMcQueen_Transportes.Data
{
    public class Intervencao
    {
        public enum Estado
        {
            Pendente,
            EmCurso,
            Concluido
        }
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "O estado é obrigatório.")]
        public Estado estado { get; set; }
        [Required(ErrorMessage = "A data é obrigatória.")]
        public DateTime Data { get; set; }
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        public Guid AtivoId { get; set; }
        public Ativo Ativo { get; set; }

        [Required]
        public Guid TecnicoId { get; set; }
        public Tecnico Tecnico { get; set; }
    }
}
