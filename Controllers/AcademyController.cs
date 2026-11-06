using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;
using G2AspNet.Models;

namespace G2AspNet.Controllers;

public class AcademyController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(string? search, string? city)
    {
        var query = context.Academies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            // EF Core no traduce Contains con StringComparison a SQL (daba error 500). Pasando ambos
            // textos a minúsculas se traduce y la búsqueda sigue ignorando mayúsculas y minúsculas.
            query = query.Where(a => a.Name.ToLower().Contains(search.ToLower()));
        }

        if (city != null) query = query.Where(a => a.City == city);

        ViewBag.Search = search;
        ViewBag.City = city;

        var academies = await query.OrderBy(a => a.Name).ToListAsync();
        return View(academies);
    }
    public async Task<IActionResult> Details(int id)
    {
        var academy = await context.Academies
        .Include(a => a.Courses)
        .Include(a => a.Classrooms)
        .FirstOrDefaultAsync(a => a.Id == id);
        
        if (academy == null)
        {
            return NotFound();
        }

        return View(academy);
    }

    // ========================================================================================
    // EJEMPLO COMPLETO (issues #17 y #18): Create, Edit, Save y Delete de Academy.
    // Es el modelo para copiar en el resto de controladores. Los pasos son siempre los mismos:
    //   1) Create y Edit (GET) solo MUESTRAN el formulario (Views/Academy/Form.cshtml).
    //   2) Save (POST) recibe el formulario, lo valida y lo guarda (crea o actualiza).
    //   3) Delete (POST) borra, pero antes comprueba que no haya datos que dependan de la fila.
    // ========================================================================================

    // GET /Academy/Create: muestra el formulario vacío.
    // Crear y editar usan la MISMA vista (Form): lo único que cambia es el modelo que le pasamos.
    public IActionResult Create()
    {
        return View("Form", new Academy());
    }

    // GET /Academy/Edit/5: muestra el mismo formulario, pero relleno con los datos del centro.
    public async Task<IActionResult> Edit(int id)
    {
        var academy = await context.Academies.FindAsync(id);

        // Si el Id no existe, devolvemos 404 en lugar de romper la página.
        if (academy == null)
        {
            return NotFound();
        }

        return View("Form", academy);
    }

    // POST /Academy/Save: recibe el formulario. Sirve para crear (Id == 0) y para editar (Id != 0).
    // [ValidateAntiForgeryToken] comprueba el token oculto que el formulario añade solo (evita envíos falsificados).
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Academy academy)
    {
        // ModelState.IsValid es false si algún campo incumple los atributos del modelo ([Required], [StringLength]...).
        // En ese caso volvemos al formulario con los datos escritos y los mensajes de error junto a cada campo.
        if (!ModelState.IsValid)
        {
            return View("Form", academy);
        }

        if (academy.Id == 0)
        {
            // Id == 0 significa "centro nuevo": lo añadimos y la base de datos le asigna el Id al guardar.
            context.Academies.Add(academy);
        }
        else
        {
            // Id != 0 significa "centro existente": buscamos la fila real y copiamos los campos del formulario.
            var existing = await context.Academies.FindAsync(academy.Id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = academy.Name;
            existing.Description = academy.Description;
            existing.Address = academy.Address;
            existing.City = academy.City;
            existing.Phone = academy.Phone;
        }

        await context.SaveChangesAsync();

        // TempData sobrevive a UNA redirección: _Layout.cshtml lo enseña como mensaje verde en la lista.
        TempData["Message"] = "Centro guardado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST /Academy/Delete/5: borra el centro, salvo que tenga aulas o cursos.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var academy = await context.Academies.FindAsync(id);

        if (academy == null)
        {
            return NotFound();
        }

        // Courses y Classrooms apuntan a Academy con OnDelete(Restrict): la base de datos NO deja borrar
        // un centro que tenga cursos o aulas y, si lo intentáramos, saldría un error 500.
        // Por eso lo comprobamos ANTES con AnyAsync y, si no se puede borrar, avisamos con un mensaje rojo.
        var hasCourses = await context.Courses.AnyAsync(c => c.AcademyId == id);
        var hasClassrooms = await context.Classrooms.AnyAsync(c => c.AcademyId == id);

        if (hasCourses || hasClassrooms)
        {
            TempData["Error"] = $"No se puede eliminar «{academy.Name}» porque todavía tiene cursos o aulas. "
                + "Mueve o borra primero sus cursos y aulas.";
            return RedirectToAction(nameof(Index));
        }

        context.Academies.Remove(academy);
        await context.SaveChangesAsync();

        TempData["Message"] = $"Centro «{academy.Name}» eliminado.";
        return RedirectToAction(nameof(Index));
    }
}
