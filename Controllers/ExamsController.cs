using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.Utilities;

namespace G2AspNet.Controllers;

public class ExamsController : Controller
{
    private readonly ApplicationDbContext context;

    public ExamsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    // Listado de exámenes.
    // Un admin o profesor ve todos; un alumno solo ve los exámenes activos.
    [Authorize]
    public IActionResult Index()
    {
        var query = context.Exams
            .Include(e => e.Course)
            .Include(e => e.Teacher)
            .AsQueryable();

        if (!User.IsInRole(RoleNames.Admin) && !User.IsInRole(RoleNames.Teacher))
        {
            query = query.Where(e => e.IsActive);
        }

        var exams = query.OrderByDescending(e => e.CreatedAt).ToList();
        return View(exams);
    }

    // Detalle de un examen con sus preguntas.
    [Authorize]
    [HttpGet]
    public IActionResult Details(int id)
    {
        var exam = context.Exams
            .Include(e => e.Course)
            .Include(e => e.Teacher)
            .Include(e => e.Questions)
            .FirstOrDefault(e => e.Id == id);

        if (exam == null)
        {
            return NotFound();
        }

        var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher);
        if (!isStaff && exam.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        return View(exam);
    }

    // Formulario para crear un nuevo examen (con borrador de preguntas acumuladas).
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Create()
    {
        var examJson = TempData.Peek("ExamBorrador") as string;
        var preguntasJson = TempData.Peek("PreguntasBorrador") as string;

        Exam exam;
        List<Question> preguntas;

        if (!string.IsNullOrEmpty(examJson) && !string.IsNullOrEmpty(preguntasJson))
        {
            exam = JsonSerializer.Deserialize<Exam>(examJson) ?? new Exam();
            preguntas = JsonSerializer.Deserialize<List<Question>>(preguntasJson) ?? new List<Question>();
        }
        else
        {
            exam = new Exam();
            preguntas = new List<Question>();
        }

        ViewBag.Preguntas = preguntas;
        ViewBag.PreguntaActual = new Question { ExamId = exam.Id };
        ViewBag.EsPrimeraPregunta = preguntas.Count == 0;

        ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
        return View("Form", exam);
    }

    // Cancela la creación del examen y limpia el borrador de TempData.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    public IActionResult Cancelar()
    {
        TempData.Remove("ExamBorrador");
        TempData.Remove("PreguntasBorrador");
        return RedirectToAction("Index");
    }

    // Formulario para editar un examen existente.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var exam = context.Exams.Include(e => e.Questions).FirstOrDefault(e => e.Id == id);
        if (exam == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && exam.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
        ViewBag.Preguntas = exam.Questions ?? new List<Question>();
        ViewBag.PreguntaActual = new Question { ExamId = exam.Id };
        ViewBag.EsPrimeraPregunta = (exam.Questions ?? new List<Question>()).Count == 0;

        return View("Form", exam);
    }

    // Guarda una pregunta en el borrador, cancela una pregunta, o guarda el examen completo.
    // accion="saveQuestion": acumula la pregunta actual en el borrador.
    // accion="cancelQuestion": descarta la pregunta actual (no se guarda).
    // accion="addQuestion": reinicia el formulario de pregunta (descarta la actual si no se guardó).
    // accion="saveExam": valida y guarda el examen con todas sus preguntas en la base de datos.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Save(Exam exam, Question pregunta, string accion)
    {
        if (accion == "saveQuestion")
        {
            // Guardar datos del examen tal como están
            TempData["ExamBorrador"] = JsonSerializer.Serialize(exam);

            // Leer preguntas existentes del borrador (sin marcar para borrar)
            var preguntasJson = TempData.Peek("PreguntasBorrador") as string;
            var preguntas = string.IsNullOrEmpty(preguntasJson)
                ? new List<Question>()
                : JsonSerializer.Deserialize<List<Question>>(preguntasJson) ?? new List<Question>();

            // Agregar la pregunta actual si tiene texto
            if (pregunta != null && !string.IsNullOrWhiteSpace(pregunta.Text))
            {
                preguntas.Add(pregunta);
            }

            TempData["PreguntasBorrador"] = JsonSerializer.Serialize(preguntas);

            return RedirectToAction("Create");
        }

        if (accion == "cancelQuestion" || accion == "addQuestion")
        {
            // Mantener los datos del examen, descartar pregunta actual
            TempData["ExamBorrador"] = JsonSerializer.Serialize(exam);
            return RedirectToAction("Create");
        }

        // accion == "saveExam"
        ModelState.Remove(nameof(Exam.TeacherId));

        // Limpiar errores del modelo de pregunta para que no bloqueen el guardado del examen
        var keysToRemove = ModelState.Keys.Where(k => k.StartsWith("pregunta.")).ToList();
        foreach (var key in keysToRemove)
        {
            ModelState.Remove(key);
        }

        if (!ModelState.IsValid)
        {
            var preguntasJson = TempData.Peek("PreguntasBorrador") as string;
            var preguntas = string.IsNullOrEmpty(preguntasJson)
                ? new List<Question>()
                : JsonSerializer.Deserialize<List<Question>>(preguntasJson) ?? new List<Question>();

            ViewBag.Preguntas = preguntas;
            ViewBag.PreguntaActual = pregunta ?? new Question { ExamId = exam.Id };
            ViewBag.EsPrimeraPregunta = preguntas.Count == 0;
            ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
            return View("Form", exam);
        }

        // Verificar que hay al menos una pregunta guardada
        var preguntasBorradorJson = TempData.Peek("PreguntasBorrador") as string;
        var preguntasBorrador = string.IsNullOrEmpty(preguntasBorradorJson)
            ? new List<Question>()
            : JsonSerializer.Deserialize<List<Question>>(preguntasBorradorJson) ?? new List<Question>();

        if (preguntasBorrador.Count == 0)
        {
            ModelState.AddModelError("", "El examen debe tener al menos una pregunta guardada.");
            ViewBag.Preguntas = preguntasBorrador;
            ViewBag.PreguntaActual = new Question { ExamId = exam.Id };
            ViewBag.EsPrimeraPregunta = true;
            ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
            return View("Form", exam);
        }

        if (exam.Id == 0)
        {
            exam.TeacherId = User.GetRequiredUserId();
            exam.CreatedAt = DateTime.UtcNow;
            context.Exams.Add(exam);
            context.SaveChanges(); // Para obtener el Id generado
        }
        else
        {
            var existing = context.Exams.Include(e => e.Questions).FirstOrDefault(e => e.Id == exam.Id);
            if (existing == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole(RoleNames.Admin);
            if (!isAdmin && existing.TeacherId != User.GetRequiredUserId())
            {
                return Forbid();
            }

            existing.Title = exam.Title;
            existing.Description = exam.Description;
            existing.CourseId = exam.CourseId;
            existing.IsActive = exam.IsActive;

            // Borrar preguntas existentes y volver a crearlas
            context.Questions.RemoveRange(existing.Questions);
            exam = existing;
        }

        // Guardar preguntas del borrador
        foreach (var p in preguntasBorrador)
        {
            p.ExamId = exam.Id;
            context.Questions.Add(p);
        }

        context.SaveChanges();

        // Limpiar borrador de TempData
        TempData.Remove("ExamBorrador");
        TempData.Remove("PreguntasBorrador");

        TempData["Message"] = "Examen guardado correctamente.";
        return RedirectToAction("Index");
    }

    // Elimina un examen y sus preguntas asociadas.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var exam = context.Exams.Include(e => e.Questions).FirstOrDefault(e => e.Id == id);
        if (exam == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && exam.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        context.Exams.Remove(exam);
        context.SaveChanges();

        TempData["Message"] = "Examen eliminado correctamente.";
        return RedirectToAction("Index");
    }
}
