using System.ComponentModel.DataAnnotations;
namespace EXAMEN_PARCIAL.Models;

public class Matricula
{
    public int Id { get; set; }

    public int CursoId { get; set; }
    public Curso? Curso { get; set; }

    public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser? Usuario { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
}

public enum EstadoMatricula
{
    Pendiente,
    Confirmada,
    Cancelada
}