using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EXAMEN_PARCIAL.Models;



namespace EXAMEN_PARCIAL.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Curso> Cursos { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configuración de Curso
        builder.Entity<Curso>(entity =>
        {
            entity.HasIndex(c => c.Codigo).IsUnique();
            entity.Property(c => c.Codigo).IsRequired().HasMaxLength(10);
            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            entity.HasCheckConstraint("CK_Curso_Creditos", "Creditos > 0");
            entity.HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");
        });

        // Configuración de Matricula
        builder.Entity<Matricula>(entity =>
        {
            entity.HasIndex(m => new { m.CursoId, m.UsuarioId }).IsUnique();
            entity.Property(m => m.Estado).HasConversion<string>();
        });
    }




}
