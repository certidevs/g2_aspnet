using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        // Solo el profesor dueño, un admin o un alumno pueden verlo.
        var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher);
        if (!isStaff && exam.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        return View(exam);
    }

    // Formulario para crear un nuevo examen.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Create()
    {
        ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
        return View("Form", new Exam());
    }

    // Formulario para editar un examen existente.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var exam = context.Exams.Find(id);
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
        return View("Form", exam);
    }

    // Guarda un examen (crea o actualiza según Id).
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Save(Exam exam)
    {
        ModelState.Remove(nameof(Exam.TeacherId));

        if (!ModelState.IsValid)
        {
            ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
            return View("Form", exam);
        }

        if (exam.Id == 0)
        {
            exam.TeacherId = User.GetRequiredUserId();
            exam.CreatedAt = DateTime.UtcNow;
            context.Exams.Add(exam);
        }
        else
        {
            var existing = context.Exams.Find(exam.Id);
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
        }

        context.SaveChanges();

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
