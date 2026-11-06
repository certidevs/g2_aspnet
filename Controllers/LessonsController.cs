using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList (desplegables), lo usa el bloque comentado de abajo
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;
using G2AspNet.Models; // la clase Lesson, la usa el bloque comentado de abajo

namespace G2AspNet.Controllers;

public class LessonsController : Controller
{
    private readonly ApplicationDbContext context;

    public LessonsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    // Lista todas las lecciones con su curso y su aula, de la más antigua a la más reciente.
    public async Task<IActionResult> Index()
    {
        var lessons = await context.Lessons
            .Include(l => l.Course)
            .Include(l => l.Classroom)
            .OrderBy(l => l.StartDate)
            .ToListAsync();

        return View(lessons);
    }

    // Detalle de una lección con su curso, su aula y las asistencias que se han apuntado.
    public async Task<IActionResult> Details(int id)
    {
        var lesson = await context.Lessons
            .Include(l => l.Course)
            .Include(l => l.Classroom)
            .Include(l => l.Attendances).ThenInclude(a => a.Enrollment).ThenInclude(e => e!.Student)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (lesson == null)
        {
            return NotFound();
        }

        return View(lesson);
    }

    // TODO (issue #25): Create, Edit, Save y Delete con su vista Form (issue #26).
    // Tienes un ejemplo completo y funcionando en AcademyController (Create, Edit, Save y Delete).

    // TODO (issue #25): Create, Edit, Save y Delete
    // Pasos: 1) borra la línea «/* DESCOMENTA DESDE AQUÍ» y la línea «FIN DESCOMENTA */», 2) ajusta los campos de tu entidad, 3) descomenta Views/Lessons/Form.cshtml, 4) descomenta los botones de Index y Details.
    /* DESCOMENTA DESDE AQUÍ

    // Rellena los dos desplegables del formulario: el curso (obligatorio) y el aula (opcional).
    private async Task LoadCoursesAndClassrooms()
    {
        var courses = await context.Courses.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Courses = new SelectList(courses, "Id", "Name");

        var classrooms = await context.Classrooms.OrderBy(c => c.Name).ToListAsync();
        ViewBag.Classrooms = new SelectList(classrooms, "Id", "Name");
    }

    // GET /Lessons/Create: muestra el formulario vacío (la misma vista Form sirve para crear y editar).
    public async Task<IActionResult> Create()
    {
        // Proponemos una lección de dos horas que empieza mañana a las 9:00 para que las fechas no salgan vacías.
        var lesson = new Lesson
        {
            StartDate = DateTime.Today.AddDays(1).AddHours(9),
            EndDate = DateTime.Today.AddDays(1).AddHours(11)
        };

        await LoadCoursesAndClassrooms();
        return View("Form", lesson);
    }

    // GET /Lessons/Edit/5: muestra el formulario relleno con los datos de la lección.
    public async Task<IActionResult> Edit(int id)
    {
        var lesson = await context.Lessons.FindAsync(id);

        if (lesson == null)
        {
            return NotFound();
        }

        await LoadCoursesAndClassrooms();
        return View("Form", lesson);
    }

    // POST /Lessons/Save: crea (Id == 0) o actualiza (Id != 0) una lección.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Lesson lesson)
    {
        // El curso debe existir (el desplegable envía 0 si no se ha elegido ninguno).
        var course = await context.Courses.FindAsync(lesson.CourseId);

        if (course == null)
        {
            ModelState.AddModelError(nameof(Lesson.CourseId), "Selecciona un curso.");
        }

        // El aula es opcional (ClassroomId es int?): solo la comprobamos si se ha elegido una.
        if (lesson.ClassroomId != null)
        {
            var classroom = await context.Classrooms.FindAsync(lesson.ClassroomId);

            if (classroom == null)
            {
                ModelState.AddModelError(nameof(Lesson.ClassroomId), "El aula elegida no existe.");
            }
            else if (course != null && classroom.AcademyId != course.AcademyId)
            {
                ModelState.AddModelError(nameof(Lesson.ClassroomId), "El aula debe ser del mismo centro que el curso.");
            }
        }

        // Además de los atributos ([Required], [StringLength]...), la validación incluye la regla de la clase:
        // la hora de fin debe ser posterior a la de inicio. Si algo falla, volvemos al formulario.
        if (!ModelState.IsValid)
        {
            await LoadCoursesAndClassrooms();
            return View("Form", lesson);
        }

        if (lesson.Id == 0)
        {
            context.Lessons.Add(lesson);
        }
        else
        {
            // Buscamos la fila real y copiamos los campos del formulario.
            var existing = await context.Lessons.FindAsync(lesson.Id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.Name = lesson.Name;
            existing.Description = lesson.Description;
            existing.StartDate = lesson.StartDate;
            existing.EndDate = lesson.EndDate;
            existing.CourseId = lesson.CourseId;
            existing.ClassroomId = lesson.ClassroomId;
        }

        await context.SaveChangesAsync();

        TempData["Message"] = "Lección guardada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // POST /Lessons/Delete/5: borra la lección.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var lesson = await context.Lessons.FindAsync(id);

        if (lesson == null)
        {
            return NotFound();
        }

        // Attendance -> Lesson es OnDelete(Cascade): al borrar la lección se borran también sus asistencias.
        // Contamos esas asistencias solo para avisar de lo que va a pasar.
        var attendanceCount = await context.Attendances.CountAsync(a => a.LessonId == id);

        context.Lessons.Remove(lesson);
        await context.SaveChangesAsync();

        TempData["Message"] = attendanceCount == 0
            ? $"Lección «{lesson.Name}» eliminada."
            : $"Lección «{lesson.Name}» eliminada. Asistencias borradas con ella: {attendanceCount}.";
        return RedirectToAction(nameof(Index));
    }

    FIN DESCOMENTA */
}
