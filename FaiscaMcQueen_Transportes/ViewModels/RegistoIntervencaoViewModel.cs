using FaiscaMcQueen_Transportes.Data.FaiscaMcQueen;
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
    public Intervencao.estado Estado { get; set; } = Intervencao.estado.Pendente;

    public List<SelectListItem>? Ativos { get; set; }
    public List<SelectListItem>? Tecnicos { get; set; }
    }
}
