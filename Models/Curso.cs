using System.ComponentModel.DataAnnotations;
namespace EXAMEN_PARCIAL.Models;

public class Curso
{
    public int Id { get; set; }

    [Required]
    [StringLength(10)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayor a 0")]
    public int Creditos { get; set; }

    [Range(1, int.MaxValue)]
    public int CupoMaximo { get; set; }

    [DataType(DataType.Time)]
    public TimeSpan HorarioInicio { get; set; }

    [DataType(DataType.Time)]
    public TimeSpan HorarioFin { get; set; }

    public bool Activo { get; set; } = true;

    // Navigation property
    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
}