using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;


namespace FaiscaMcQueen_Transportes.Data.FaiscaMcQueen
{
    public class Intervencao
    {
        public enum estado
        {
            Pendente,
            [Display(Name = "Em Curso")]
            EmCurso,
            [Display(Name = "Concluído")]
            Concluido
        }
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "O estado é obrigatório.")]
  
        public estado Estado { get; set; }

        [Required(ErrorMessage = "A data de criação é obrigatória.")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime DataInicio { get; set; }

        public DateTime? DataFim { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; } = string.Empty;

        public Guid? AtivoId { get; set; }
        public virtual Ativo Ativo { get; set; }
        public Guid? TecnicoId { get; set; }
        public virtual Tecnico Tecnico { get; set; }
    }
}
