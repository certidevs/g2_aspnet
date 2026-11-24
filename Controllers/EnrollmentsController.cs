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


    [Authorize(Roles = RoleNames.Student)]
    [HttpPost]
    public IActionResult Enroll(int courseId)
    {
        var course = context.Courses.Find(courseId);
        if (course == null)
        {
            return NotFound();
        }

        if (course.Status != CourseStatus.Open)
        {
            TempData["Error"] = "Las matrículas de este curso están cerradas.";
            return RedirectToAction("Details", "Courses", new { id = course.Id });
        }

        var userId = User.GetRequiredUserId();
        var alreadyEnrolled = context.Enrollments.Any(e => e.CourseId == courseId
            && e.StudentId == userId
            && EnrollmentRules.PlaceTakingStatuses.Contains(e.Status));
        if (alreadyEnrolled)
        {
            TempData["Error"] = "Ya estás matriculado en este curso.";
            return RedirectToAction("Details", "Courses", new { id = course.Id });
        }

        var occupied = context.Enrollments
            .Count(e => e.CourseId == courseId && EnrollmentRules.PlaceTakingStatuses.Contains(e.Status));
        if (occupied >= course.MaxStudents)
        {
            TempData["Error"] = "Curso completo: no quedan plazas.";
            return RedirectToAction("Details", "Courses", new { id = course.Id });
        }

        var enrollment = new Enrollment
        {
            StudentId = userId,
            CourseId = course.Id,
            EnrolledAt = DateTime.UtcNow,
            IsPaid = false,
            Status = EnrollmentStatus.Pending
        };
        context.Enrollments.Add(enrollment);
        context.SaveChanges();

        TempData["Message"] = "Matrícula realizada. Queda pendiente de pago.";
        return RedirectToAction("Details", new { id = enrollment.Id });
    }

    [Authorize]
    [HttpPost]
    public IActionResult Pay(int id)
    {
        var enrollment = VisibleEnrollments().FirstOrDefault(e => e.Id == id);
        if (enrollment == null)
        {
            return NotFound();
        }

        if (enrollment.Status != EnrollmentStatus.Pending)
        {
            TempData["Error"] = "Solo se puede pagar una matrícula pendiente.";
            return RedirectToAction("Details", new { id });
        }

        enrollment.IsPaid = true;
        enrollment.Status = EnrollmentStatus.Active;
        context.SaveChanges();

        TempData["Message"] = "Pago registrado. Tu matrícula está activa.";
        return RedirectToAction("Details", new { id });
    }

    [Authorize]
    [HttpPost]
    public IActionResult Drop(int id)
    {
        var enrollment = VisibleEnrollments().FirstOrDefault(e => e.Id == id);
        if (enrollment == null)
        {
            return NotFound();
        }

        if (!EnrollmentRules.TakesPlace(enrollment))
        {
            TempData["Error"] = "Solo se puede abandonar una matrícula pendiente o activa.";
            return RedirectToAction("Details", new { id });
        }

        enrollment.Status = EnrollmentStatus.Dropped;
        context.SaveChanges();

        TempData["Message"] = "Has abandonado el curso. Tu plaza queda libre.";
        return RedirectToAction("Details", new { id });
    }


    private void LoadCourses()
    {
        ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
    }

    /* [Authorize]
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
    } */

    private IQueryable<Enrollment> VisibleEnrollments()
    {
        var query = context.Enrollments.AsQueryable();

        if (!User.IsInRole(RoleNames.Admin))
        {
            var userId = User.GetRequiredUserId();
            query = query.Where(e => e.StudentId == userId);
        }

        return query;
    }

    [Authorize]
    public IActionResult Index()
    {
        var enrollments = VisibleEnrollments()
            .Include(e => e.Course)
            .Include(e => e.Student)
            .OrderByDescending(e => e.EnrolledAt)
            .ToList();

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

        // Una matrícula con notas o asistencias es historial (Restrict): no se borra. Se avisa en vez de dar un error 500.
        if (context.Grades.Any(g => g.EnrollmentId == id) || context.Attendances.Any(a => a.EnrollmentId == id))
        {
            TempData["Error"] = "No se puede eliminar la matrícula porque tiene notas o asistencias.";
            return RedirectToAction("Index");
        }

        context.Enrollments.Remove(enrollment);
        context.SaveChanges();

        TempData["Message"] = "Matriculación eliminada.";
        return RedirectToAction("Index");
    }
}