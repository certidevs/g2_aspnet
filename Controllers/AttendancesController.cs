using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.ViewModels.Attendances;
using Microsoft.EntityFrameworkCore;

namespace G2AspNet.Controllers;

// El controlador se llama AttendancesController (en plural, como el resto y como las vistas de Views/Attendances).
public class AttendancesController : Controller
{
    private readonly ApplicationDbContext context;

    public AttendancesController(ApplicationDbContext context)
    {
        this.context = context;
    }

    // Lista las asistencias con su lección, el alumno y el curso.
    // La búsqueda filtra por nombre de lección, de curso o de alumno.
    public IActionResult Index(string? search)
    {
        // Include carga las relaciones: sin él, Lesson y Enrollment llegarían vacíos a la vista.
        var query = context.Attendances
            .Include(attendance => attendance.Lesson)
            .Include(attendance => attendance.Enrollment).ThenInclude(enrollment => enrollment!.Student)
            .Include(attendance => attendance.Enrollment).ThenInclude(enrollment => enrollment!.Course)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(attendance =>
                attendance.Lesson!.Name.Contains(search)
                || attendance.Enrollment!.Course!.Name.Contains(search)
                || attendance.Enrollment!.Student!.DisplayName!.Contains(search));
        }

        var model = new AttendanceIndexViewModel
        {
            Search = search,
            Attendances = query
                .OrderBy(attendance => attendance.Id)
                .ToList()
                .Select(attendance => new AttendanceListItemViewModel { Attendance = attendance })
                .ToList()
        };

        return View(model);
    }

    // Detalle de una asistencia: a qué lección, de qué alumno y de qué curso.
    public IActionResult Details(int id)
    {
        var attendance = context.Attendances
            .Include(a => a.Lesson).ThenInclude(l => l!.Course)
            .Include(a => a.Enrollment).ThenInclude(e => e!.Student)
            .Include(a => a.Enrollment).ThenInclude(e => e!.Course)
            .FirstOrDefault(a => a.Id == id);

        if (attendance == null)
        {
            return NotFound();
        }

        return View(attendance);
    }

    // Método Edit
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var attendance = context.Attendances.Find(id);
        if (attendance == null)
        {
            return NotFound();
        }
        LoadDropdowns(attendance);
        return View("Form", attendance);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var attendance = context.Attendances.Find(id);
        if (attendance == null)
        {
            return NotFound();
        }
        var lessonId = attendance.LessonId;
        context.Attendances.Remove(attendance);
        context.SaveChanges();

        TempData["Message"] = "Asistencia eliminada correctamente.";
        return RedirectToAction("Index", "Attendances", new { id = attendance.Id });
    }

    [HttpGet]
    public IActionResult Create(int lessonId, int enrollmentId)
    {
        var attendance = new Attendance
        {
            LessonId = lessonId,
            EnrollmentId = enrollmentId,
            Status = AttendanceStatus.Present
        };

        LoadDropdowns(attendance);
        TempData["Message"] = "Creando nueva asistencia para la lección y matrícula seleccionadas.";
        return View("Form", attendance);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Attendance attendance)
    {
        CheckNotDuplicated(attendance);

        if (ModelState.IsValid)
        {
            context.Attendances.Add(attendance);
            context.SaveChanges();
            TempData["Message"] = "Asistencia creada correctamente.";
            return RedirectToAction("Details", "Lessons", new { id = attendance.LessonId });
        }

        LoadDropdowns(attendance);
        return View("Form", attendance);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Save(Attendance attendance)
    {
        CheckNotDuplicated(attendance);

        if (ModelState.IsValid)
        {
            context.Attendances.Update(attendance);
            context.SaveChanges();
            return RedirectToAction("Details", "Lessons", new { id = attendance.LessonId });
        }

        LoadDropdowns(attendance);
        TempData["Message"] = "Asistencia actualizada correctamente.";
        return View("Form", attendance);
    }

    // Hay un índice único (LessonId + EnrollmentId): una matrícula solo puede tener una asistencia por lección.
    // Lo comprobamos antes de guardar para mostrar un mensaje en el formulario en vez de un error 500.
    private void CheckNotDuplicated(Attendance attendance)
    {
        var exists = context.Attendances.Any(a =>
            a.LessonId == attendance.LessonId
            && a.EnrollmentId == attendance.EnrollmentId
            && a.Id != attendance.Id);

        if (exists)
        {
            ModelState.AddModelError(nameof(Attendance.EnrollmentId), "Esa matrícula ya tiene asistencia registrada en esa lección.");
        }
    }

    // Rellena los dos desplegables del formulario: la lección y la matrícula (alumno + curso).
    private void LoadDropdowns(Attendance attendance)
    {
        var lessons = context.Lessons
            .Include(l => l.Course)
            .OrderBy(l => l.StartDate)
            .ToList()
            .Select(l => new { l.Id, Text = $"{l.Course?.Name} · {l.Name} ({l.StartDate:dd/MM/yyyy})" });
        ViewBag.Lessons = new SelectList(lessons, "Id", "Text", attendance.LessonId);

        var enrollments = context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .OrderBy(e => e.Id)
            .ToList()
            .Select(e => new { e.Id, Text = $"{e.Student?.DisplayName ?? e.Student?.UserName} · {e.Course?.Name}" });
        ViewBag.Enrollments = new SelectList(enrollments, "Id", "Text", attendance.EnrollmentId);
    }
}
