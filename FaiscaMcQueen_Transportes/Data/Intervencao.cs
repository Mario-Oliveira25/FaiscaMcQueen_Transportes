using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

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
        [Required]
        public Estado estado { get; set; }
        [Required]
        public DateTime Data { get; set; }
        [Required]
        public string Descricao { get; set; } = string.Empty;
        [Required]
        public Guid AtivoId { get; set; }
        public Ativo Ativo { get; set; }
        [Required]
        public Guid TecnicoId { get; set; }
        public Tecnico Tecnico { get; set; }
    }
}
