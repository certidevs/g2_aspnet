using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList (desplegable del formulario)
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;
using G2AspNet.Models;
using Microsoft.AspNetCore.Authorization;
using G2AspNet.Utilities;

namespace G2AspNet.Controllers;

public class GradesController : Controller
{
    private readonly ApplicationDbContext context;

    public GradesController(ApplicationDbContext context)
    {
        this.context = context;
    }

    [Authorize]
    public IActionResult Index()
    {
        var query = context.Grades
            .Include(g => g.Enrollment).ThenInclude(e => e!.Student)
            .Include(g => g.Enrollment).ThenInclude(e => e!.Course)
            .AsQueryable();

        var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher);
        if (!isStaff)
        {
            var userId = User.GetRequiredUserId();
            query = query.Where(g => g.Enrollment!.StudentId == userId);
        }

        var grades = query.OrderBy(g => g.EnrollmentId).ToList();
        return View(grades);
    }

    // Detalle de una nota con la matrícula a la que pertenece (alumno y curso).
    [Authorize]
    [HttpGet]
    public IActionResult Details(int id)
    {
        var grade = context.Grades
            .Include(g => g.Enrollment).ThenInclude(e => e!.Student)
            .Include(g => g.Enrollment).ThenInclude(e => e!.Course)
            .FirstOrDefault(g => g.Id == id);

        if (grade == null)
        {
            return NotFound();
        }

        // Igual que en Index: el profesorado ve todas las notas y un alumno solo las suyas.
        var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher);
        if (!isStaff && grade.Enrollment?.StudentId != User.GetRequiredUserId())
        {
            return NotFound();
        }

        return View(grade);
    }

    // Rellena el desplegable del formulario con las matrículas (alumno + curso).
    // Antes era un campo numérico donde había que teclear el Id: un Id que no existe daba un error 500.
    private void LoadEnrollments(Grade grade)
    {
        var enrollments = context.Enrollments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .OrderBy(e => e.Id)
            .ToList()
            .Select(e => new { e.Id, Text = $"{e.Student?.DisplayName ?? e.Student?.UserName} · {e.Course?.Name}" });
        ViewBag.Enrollments = new SelectList(enrollments, "Id", "Text", grade.EnrollmentId);
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Create()
    {
        var grade = new Grade();
        LoadEnrollments(grade);
        return View("Form", grade);
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var grade = context.Grades.Find(id);
        if (grade == null)
        {
            return NotFound();
        }

        LoadEnrollments(grade);
        return View("Form", grade);
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Save(Grade grade)
    {
        // La matrícula tiene que existir: si no, la base de datos rechaza la clave foránea (error 500).
        if (!context.Enrollments.Any(e => e.Id == grade.EnrollmentId))
        {
            ModelState.AddModelError(nameof(Grade.EnrollmentId), "Selecciona una matrícula.");
        }

        if (!ModelState.IsValid)
        {
            LoadEnrollments(grade);
            return View("Form", grade);
        }

        if (grade.Id == 0)
        {
            context.Grades.Add(grade);
        }
        else
        {
            context.Grades.Update(grade);
        }

        context.SaveChanges();

        TempData["Message"] = "Nota guardada correctamente.";
        return RedirectToAction("Index");
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var grade = context.Grades.Find(id);
        if (grade == null)
        {
            return NotFound();
        }

        context.Grades.Remove(grade);
        context.SaveChanges();

        TempData["Message"] = "Nota borrada.";
        return RedirectToAction("Index");
    }
}