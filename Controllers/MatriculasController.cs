using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EXAMEN_PARCIAL.Data;
using EXAMEN_PARCIAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EXAMEN_PARCIAL.Controllers;

    [Authorize]
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Matriculas/Create
        public async Task<IActionResult> Create(int cursoId)
        {
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == cursoId && c.Activo);

            if (curso == null)
            {
                return NotFound();
            }

            // Verificar si el usuario ya está matriculado en este curso
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var matriculaExistente = await _context.Matriculas
                .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == userId && m.Estado != EstadoMatricula.Cancelada);

            if (matriculaExistente)
            {
                TempData["Error"] = "Ya estás matriculado en este curso.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            // Verificar cupo disponible
            var cupoOcupado = await _context.Matriculas
                .CountAsync(m => m.CursoId == cursoId && m.Estado == EstadoMatricula.Confirmada);

            if (cupoOcupado >= curso.CupoMaximo)
            {
                TempData["Error"] = "El curso ha alcanzado su cupo máximo.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            // Verificar superposición de horarios
            var matriculasActivas = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.UsuarioId == userId && m.Estado == EstadoMatricula.Confirmada)
                .ToListAsync();

            var conflicto = matriculasActivas.Any(m =>
                (curso.HorarioInicio < m.Curso.HorarioFin && curso.HorarioFin > m.Curso.HorarioInicio));

            if (conflicto)
            {
                TempData["Error"] = "El horario de este curso se superpone con otro curso en el que estás matriculado.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            var matricula = new Matricula
            {
                CursoId = cursoId,
                Curso = curso
            };

            return View(matricula);
        }

        // POST: Matriculas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CursoId")] Matricula matricula)
        {
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == matricula.CursoId && c.Activo);

            if (curso == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Validaciones server-side (repetimos por si acaso)
            // 1. Verificar si ya está matriculado
            var matriculaExistente = await _context.Matriculas
                .AnyAsync(m => m.CursoId == matricula.CursoId && m.UsuarioId == userId && m.Estado != EstadoMatricula.Cancelada);

            if (matriculaExistente)
            {
                ModelState.AddModelError(string.Empty, "Ya estás matriculado en este curso.");
            }

            // 2. Verificar cupo
            var cupoOcupado = await _context.Matriculas
                .CountAsync(m => m.CursoId == matricula.CursoId && m.Estado == EstadoMatricula.Confirmada);

            if (cupoOcupado >= curso.CupoMaximo)
            {
                ModelState.AddModelError(string.Empty, "El curso ha alcanzado su cupo máximo.");
            }

            // 3. Verificar superposición de horarios
            var matriculasActivas = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.UsuarioId == userId && m.Estado == EstadoMatricula.Confirmada)
                .ToListAsync();

            var conflicto = matriculasActivas.Any(m =>
                (curso.HorarioInicio < m.Curso.HorarioFin && curso.HorarioFin > m.Curso.HorarioInicio));

            if (conflicto)
            {
                ModelState.AddModelError(string.Empty, "El horario de este curso se superpone con otro curso en el que estás matriculado.");
            }

            if (ModelState.IsValid)
            {
                matricula.UsuarioId = userId;
                matricula.FechaRegistro = DateTime.Now;
                matricula.Estado = EstadoMatricula.Pendiente;

                _context.Add(matricula);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Matrícula guardada: ID {matricula.Id}, CursoId {matricula.CursoId}, UsuarioId {matricula.UsuarioId}");

                TempData["Success"] = "Tu matrícula se ha registrado correctamente. Está pendiente de confirmación.";
                return RedirectToAction("Details", "Cursos", new { id = matricula.CursoId });
            }

            // Si hay errores, recargar el curso para mostrar en la vista
            matricula.Curso = curso;
            return View(matricula);
        }
    }
