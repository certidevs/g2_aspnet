using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList (desplegables del formulario)
using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.ViewModels.Courses;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Utilities;

namespace G2AspNet.Controllers;

public class CoursesController : Controller
{
    private readonly ApplicationDbContext context;

    public CoursesController(ApplicationDbContext context)
    {
        this.context = context;
    }

    private IQueryable<Course> ManageableCourses()
    {
        var query = context.Courses.AsQueryable();

        if (User.IsInRole(RoleNames.Admin))
        {
            return query;
        }

        var userId = User.GetRequiredUserId();
        return query.Where(c => c.TeacherId == userId);
    }

    public IActionResult Index(string? search)
    {
        var query = context.Courses.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(course => course.Name.Contains(search));
        }

        var model = new CourseIndexViewModel
        {
            Search = search,
            Courses = query
                .OrderBy(course => course.Name)
                .Select(course => new CourseListItemViewModel { Course = course })
                .ToList()
        };

        return View(model);
    }

    // Rellena los desplegables del formulario: el centro (obligatorio) y el profesor (opcional).
    // Sin el desplegable del centro, AcademyId llegaba como 0, la clave foránea fallaba y daba un error 500.
    private void LoadAcademiesAndTeachers(Course course)
    {
        ViewBag.Academies = new SelectList(context.Academies.OrderBy(a => a.Name).ToList(), "Id", "Name", course.AcademyId);

        // Profesores = usuarios con el rol Teacher (se unen las tablas de usuarios, roles de usuario y roles).
        var teachers = (from user in context.Users
                        join userRole in context.UserRoles on user.Id equals userRole.UserId
                        join role in context.Roles on userRole.RoleId equals role.Id
                        where role.Name == RoleNames.Teacher
                        select user).ToList();
        ViewBag.Teachers = new SelectList(
            teachers.OrderBy(t => t.DisplayName).Select(t => new { t.Id, Name = t.DisplayName ?? t.UserName }),
            "Id", "Name", course.TeacherId);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Create()
    {
        var course = new Course();
        LoadAcademiesAndTeachers(course);
        return View("Form", course);
    }

    [HttpGet]
    [Authorize]
    public IActionResult Edit(int id)
    {
        var course = context.Courses.Find(id);
        if (course == null)
        {
            return NotFound();
        }

        LoadAcademiesAndTeachers(course);
        return View("Form", course);
    }

    [HttpGet]
    public IActionResult Details(int id)
    {
        var course = context.Courses.Include(c => c.Academy).Include(c => c.Teacher).FirstOrDefault(c => c.Id == id);
        if (course == null)
        {
            return NotFound();
        }

        ViewBag.OccupiedPlaces = context.Enrollments
            .Count(e => e.CourseId == id && EnrollmentRules.PlaceTakingStatuses.Contains(e.Status));

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.GetRequiredUserId();
            ViewBag.MyEnrollment = context.Enrollments.FirstOrDefault(e => e.CourseId == id
                && e.StudentId == userId
                && EnrollmentRules.PlaceTakingStatuses.Contains(e.Status));
        }

        var canManage = (User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher))
            && ManageableCourses().Any(c => c.Id == id);
        ViewBag.CanManage = canManage;
        if (canManage)
        {
            ViewBag.CourseEnrollments = context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.CourseId == id)
                .OrderBy(e => e.EnrolledAt)
                .ToList();
        }

        return View("Details", course);
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Start(int id)
    {
        var course = ManageableCourses()
            .Include(c => c.Enrollments)
            .FirstOrDefault(c => c.Id == id);
        if (course == null)
        {
            return NotFound();
        }

        if (course.Status != CourseStatus.Open)
        {
            TempData["Error"] = "Solo se puede iniciar un curso abierto.";
            return RedirectToAction("Details", new { id });
        }

        if (!course.Enrollments.Any(e => e.Status == EnrollmentStatus.Active))
        {
            TempData["Error"] = "No se puede iniciar un curso sin ningún alumno con la matrícula activa.";
            return RedirectToAction("Details", new { id });
        }

        var unpaid = course.Enrollments.Where(e => e.Status == EnrollmentStatus.Pending).ToList();
        foreach (var enrollment in unpaid)
        {
            enrollment.Status = EnrollmentStatus.Dropped;
        }

        course.Status = CourseStatus.InProgress;
        context.SaveChanges();

        if (unpaid.Count == 0)
        {
            TempData["Message"] = "Curso iniciado.";
        }
        else
        {
            TempData["Message"] = $"Curso iniciado. Matrículas sin pagar dadas de baja: {unpaid.Count}.";
        }

        return RedirectToAction("Details", new { id });
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Finish(int id)
    {
        var course = ManageableCourses()
            .Include(c => c.Enrollments)
            .FirstOrDefault(c => c.Id == id);
        if (course == null)
        {
            return NotFound();
        }

        if (course.Status != CourseStatus.InProgress)
        {
            TempData["Error"] = "Solo se puede finalizar un curso en progreso.";
            return RedirectToAction("Details", new { id });
        }

        var active = course.Enrollments.Where(e => e.Status == EnrollmentStatus.Active).ToList();
        foreach (var enrollment in active)
        {
            enrollment.Status = EnrollmentStatus.Completed;
        }

        course.Status = CourseStatus.Finished;
        context.SaveChanges();

        TempData["Message"] = $"Curso finalizado. Matrículas completadas: {active.Count}.";
        return RedirectToAction("Details", new { id });
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [ValidateAntiForgeryToken]
    public IActionResult Save(Course course)
    {
        // El centro tiene que existir: si no, la base de datos rechaza la clave foránea (error 500).
        if (!context.Academies.Any(a => a.Id == course.AcademyId))
        {
            ModelState.AddModelError(nameof(Course.AcademyId), "Selecciona un centro.");
        }

        if (!ModelState.IsValid)
        {
            LoadAcademiesAndTeachers(course);
            return View("Form", course);
        }

        if (course.Id == 0)
        {
            context.Courses.Add(course);
        }
        else
        {
            context.Courses.Update(course);
        }

        context.SaveChanges();

        TempData["Message"] = "Curso guardado correctamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var course = context.Courses.Find(id);
        if (course == null)
        {
            return NotFound();
        }

        // Un curso con matrículas es historial (Restrict): no se borra. Se avisa en vez de dar un error 500.
        if (context.Enrollments.Any(e => e.CourseId == id))
        {
            TempData["Error"] = $"No se puede borrar «{course.Name}» porque tiene matrículas.";
            return RedirectToAction("Index");
        }

        context.Courses.Remove(course);
        context.SaveChanges();

        TempData["Message"] = "Curso borrado correctamente.";
        return RedirectToAction("Index");
    }
}
