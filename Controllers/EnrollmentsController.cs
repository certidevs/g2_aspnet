using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.Utilities;

namespace G2AspNet.Controllers;

public class EnrollmentsController : Controller
{
    private readonly ApplicationDbContext context;

    public EnrollmentsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    private void LoadCourses()
    {
        ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
    }

    [Authorize]
    public IActionResult Index()
    {
        var query = context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Student)
            .AsQueryable();

        if (!User.IsInRole(RoleNames.Admin))
        {
            var userId = User.GetRequiredUserId();
            query = query.Where(e => e.StudentId == userId);
        }

        var enrollments = query.OrderByDescending(e => e.EnrolledAt).ToList();
        return View(enrollments);
    }

    // Detalle de una matrícula con su alumno, su curso, sus notas y sus asistencias.
    // De momento es público: la seguridad (quién puede ver qué matrícula) se verá más adelante.
    [Authorize]
    [HttpGet]
    public IActionResult Details(int id)
    {
        var enrollment = context.Enrollments
            .Include(e => e.Course)
            .Include(e => e.Student)
            .Include(e => e.Grades)
            .Include(e => e.Attendances).ThenInclude(a => a.Lesson)
            .FirstOrDefault(e => e.Id == id);

        if (enrollment == null)
        {
            return NotFound();
        }

        var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher);
        if (!isStaff && enrollment.StudentId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        return View(enrollment);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        LoadCourses();
        return View("Form", new Enrollment());
    }

    [Authorize]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var enrollment = context.Enrollments.Find(id);
        if (enrollment == null)
        {
            return NotFound();
        }

        if (enrollment.StudentId != User.GetRequiredUserId() && !User.IsInRole(RoleNames.Admin))
        {
            return Forbid();
        }

        LoadCourses();
        return View("Form", enrollment);
    }

    [Authorize]
    [HttpPost]
    public IActionResult Save(Enrollment enrollment)
    {
        ModelState.Remove(nameof(Enrollment.StudentId));

        if (!ModelState.IsValid)
        {
            LoadCourses();
            return View("Form", enrollment);
        }

        if (enrollment.Id == 0)
        {
            enrollment.StudentId = User.GetRequiredUserId();
            enrollment.EnrolledAt = DateTime.UtcNow;
            enrollment.IsPaid = false;
            enrollment.Status = EnrollmentStatus.Pending;
            context.Enrollments.Add(enrollment);
        }
        else
        {
            var existing = context.Enrollments.Find(enrollment.Id);
            if (existing == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole(RoleNames.Admin);
            if (existing.StudentId != User.GetRequiredUserId() && !isAdmin)
            {
                return Forbid();
            }

            existing.CourseId = enrollment.CourseId;
            if (isAdmin)
            {
                existing.IsPaid = enrollment.IsPaid;
            }
        }

        context.SaveChanges();

        TempData["Message"] = "Matriculación enviada correctamente.";
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var enrollment = context.Enrollments.Find(id);
        if (enrollment == null)
        {
            return NotFound();
        }

        if (enrollment.StudentId != User.GetRequiredUserId() && !User.IsInRole(RoleNames.Admin))
        {
            return Forbid();
        }

        context.Enrollments.Remove(enrollment);
        context.SaveChanges();

        TempData["Message"] = "Matriculación eliminada.";
        return RedirectToAction("Index");
    }
}