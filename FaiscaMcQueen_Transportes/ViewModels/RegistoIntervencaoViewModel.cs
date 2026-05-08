using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FaiscaMcQueen_Transportes.ViewModels
{
    public class RegistoIntervencaoViewModel
    {
    [Required]
    public Guid AtivoId { get; set; }

    [Required]
     public Guid TecnicoId { get; set; }

    [Required]
    public DateTime DataIntervencao { get; set; } = DateTime.Now;

    [Required]
    [StringLength(500)]
    public string Descricao { get; set; }= string.Empty;

    [Required]
    public string Estado { get; set; } = "Pendente";

    public SelectList Ativos { get; set; }
    public SelectList Tecnicos { get; set; }

    }
}
