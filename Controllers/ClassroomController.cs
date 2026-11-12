using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList (desplegables), lo usa el bloque comentado de abajo
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;
using G2AspNet.Models;

namespace G2AspNet.Controllers;

public class ClassroomController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var classroom = await context.Classrooms
        .Include(c => c.Academy)
        .OrderBy(c => c.Name)
        .ToListAsync();

        return View(classroom);
    }
    public async Task<IActionResult> Details(int id)
    {
        var classroom = await context.Classrooms
        .Include(c => c.Academy)
        .Include(c => c.Lessons)
        .FirstOrDefaultAsync(a => a.Id == id);

        if (classroom == null)
        {
            return NotFound();
        }

        return View(classroom);
    }

    // Rellena el desplegable de centros del formulario: cada aula pertenece a un centro.
    private async Task LoadAcademies()
    {
        var academies = await context.Academies
        .OrderBy(a => a.Name)
        .ToListAsync();

        ViewBag.Academies = new SelectList(academies, "Id", "Name");
    }

    // GET /Classroom/Create: muestra el formulario vacío (la misma vista Form sirve para crear y editar).
    public async Task<IActionResult> Create()
    {
        await LoadAcademies();
        return View("Form", new Classroom());
    }

    // GET /Classroom/Edit/5: muestra el formulario relleno con los datos del aula.
    public async Task<IActionResult> Edit(int id)
    {
        var classroom = await context.Classrooms.FindAsync(id);

        if (classroom == null)
        {
            return NotFound();
        }

        await LoadAcademies();
        return View("Form", classroom);
    }

    // POST /Classroom/Save: crea (Id == 0) o actualiza (Id != 0) un aula.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Classroom classroom)
    {
        // El centro debe existir (el desplegable envía 0 si no se ha elegido ninguno).
        if (!await context.Academies.AnyAsync(a => a.Id == classroom.AcademyId))
        {
            ModelState.AddModelError(nameof(Classroom.AcademyId), "Selecciona un centro.");
        }

        // Además de los atributos ([Required], [Range]...), la validación incluye la regla de la clase:
        // un aula telemática necesita enlace de reunión. Si algo falla, volvemos al formulario.
        if (!ModelState.IsValid)
        {
            await LoadAcademies();
            return View("Form", classroom);
        }

        if (classroom.Id == 0)
        {
            context.Classrooms.Add(classroom);
        }
        else
        {
            // Buscamos la fila real y copiamos los campos del formulario.
            var existing = await context.Classrooms.FindAsync(classroom.Id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = classroom.Name;
            existing.Capacity = classroom.Capacity;
            existing.ClassroomType = classroom.ClassroomType;
            existing.MeetingUrl = classroom.MeetingUrl;
            existing.IsActive = classroom.IsActive;
            existing.AcademyId = classroom.AcademyId;
        }

        await context.SaveChangesAsync();

        TempData["Message"] = "Aula guardada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST /Classroom/Delete/5: borra el aula.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var classroom = await context.Classrooms.FindAsync(id);

        if (classroom == null)
        {
            return NotFound();
        }

        // Lesson -> Classroom es OnDelete(SetNull): borrar el aula NO falla; sus lecciones se quedan "sin aula asignada".
        // Contamos esas lecciones solo para avisar de lo que va a pasar (con Restrict habría que bloquear el borrado).
        var lessonCount = await context.Lessons.CountAsync(l => l.ClassroomId == id);

        context.Classrooms.Remove(classroom);
        await context.SaveChangesAsync();

        TempData["Message"] = lessonCount == 0
            ? $"Aula «{classroom.Name}» eliminada."
            : $"Aula «{classroom.Name}» eliminada. Lecciones que se han quedado sin aula asignada: {lessonCount}.";
        return RedirectToAction(nameof(Index));
    }
}
