using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EXAMEN_PARCIAL.Models;

public class Matricula
{
    public int Id { get; set; }

    [Required]
    public int CursoId { get; set; }

    [ForeignKey("CursoId")]
    public Curso? Curso { get; set; }

    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    [ForeignKey("UsuarioId")]
    public ApplicationUser? Usuario { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [Required]
    public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
}

public enum EstadoMatricula
{
    Pendiente,
    Confirmada,
    Cancelada
}