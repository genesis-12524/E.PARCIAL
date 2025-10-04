using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EXAMEN_PARCIAL.Data;
using EXAMEN_PARCIAL.Models;
using System.Diagnostics;

namespace EXAMEN_PARCIAL.Controllers;

    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cursos
        public async Task<IActionResult> Index(string nombre, int? creditosMin, int? creditosMax, string horario)
        {
            var cursos = _context.Cursos.Where(c => c.Activo);

            if (!string.IsNullOrEmpty(nombre))
            {
                cursos = cursos.Where(c => c.Nombre.Contains(nombre));
            }

            if (creditosMin.HasValue)
            {
                cursos = cursos.Where(c => c.Creditos >= creditosMin.Value);
            }

            if (creditosMax.HasValue)
            {
                cursos = cursos.Where(c => c.Creditos <= creditosMax.Value);
            }

            if (!string.IsNullOrEmpty(horario))
            {
                // Filtro por horario: mañana, tarde, noche (ejemplo)
                switch (horario.ToLower())
                {
                    case "mañana":
                        cursos = cursos.Where(c => c.HorarioInicio >= new TimeSpan(6, 0, 0) && c.HorarioFin <= new TimeSpan(12, 0, 0));
                        break;
                    case "tarde":
                        cursos = cursos.Where(c => c.HorarioInicio >= new TimeSpan(12, 0, 0) && c.HorarioFin <= new TimeSpan(18, 0, 0));
                        break;
                    case "noche":
                        cursos = cursos.Where(c => c.HorarioInicio >= new TimeSpan(18, 0, 0) && c.HorarioFin <= new TimeSpan(23, 59, 59));
                        break;
                }
            }

            var model = await cursos.ToListAsync();
            return View(model);
        }

        // GET: Cursos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Cursos
                .FirstOrDefaultAsync(m => m.Id == id && m.Activo);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }
    }
