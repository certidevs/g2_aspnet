​
using G2AspNet.Models;

namespace G2AspNet.Utilities;

public static class EnrollmentRules
{
    public static readonly EnrollmentStatus[] PlaceTakingStatuses =
        { EnrollmentStatus.Pending, EnrollmentStatus.Active };

    public static bool TakesPlace(Enrollment enrollment) =>
        PlaceTakingStatuses.Contains(enrollment.Status);
}

---

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList (desplegables del formulario)
using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.ViewModels.Courses;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Utilities;
using Microsoft.AspNetCore.Authorization;

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
    public IActionResult Create()
    {
        var course = new Course();
        LoadAcademiesAndTeachers(course);
        return View("Form", course);
    }

    [HttpGet]
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

---

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

    [Authorize(Roles = RoleNames.Admin)]
    [HttpGet]
    public IActionResult Create()
    {
        LoadCourses();
        return View("Form", new Enrollment());
    }

    [Authorize(Roles = RoleNames.Admin)]
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

    [Authorize(Roles = RoleNames.Admin)]
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

    [Authorize(Roles = RoleNames.Admin)]
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

---

@model Enrollment
@{
    ViewData["Title"] = "Detalle de matrícula";
}

<div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
    <h1 class="h2 mb-0">Detalle de matrícula</h1>
</div>

<div class="card mb-4">
    <div class="card-body p-4">
        <dl class="row mb-0">
            <dt class="col-sm-3">Alumno</dt>
            <dd class="col-sm-9">@(Model.Student?.DisplayName ?? Model.Student?.UserName)</dd>
            <dt class="col-sm-3">Curso</dt>
            <dd class="col-sm-9">
                @if (Model.Course is not null)
                {
                    <a asp-controller="Courses" asp-action="Details" asp-route-id="@Model.Course.Id">@Model.Course.Name</a>
                }
            </dd>
            <dt class="col-sm-3">Fecha de matrícula</dt>
            <dd class="col-sm-9">@Model.EnrolledAt.ToString("dd/MM/yyyy HH:mm")</dd>
            <dt class="col-sm-3">Estado</dt>
            <dd class="col-sm-9">@Html.DisplayFor(m => m.Status)</dd>
            <dt class="col-sm-3">Pagada</dt>
            <dd class="col-sm-9">@(Model.IsPaid ? "Sí" : "No")</dd>
        </dl>
    </div>
    <div class="card-footer bg-transparent d-flex flex-wrap gap-2">
        @if (Model.Status == EnrollmentStatus.Pending)
        {
            <form asp-action="Pay" asp-route-id="@Model.Id" method="post">
                <button class="btn btn-success" type="submit">Pagar matrícula</button>
            </form>
        }

        @if (Model.Status == EnrollmentStatus.Pending || Model.Status == EnrollmentStatus.Active)
        {
            <form asp-action="Drop" asp-route-id="@Model.Id" method="post"
                  onsubmit="return confirm('¿Seguro que quieres abandonar este curso?');">
                <button class="btn btn-outline-danger" type="submit">Abandonar curso</button>
            </form>
        }

        <a class="btn btn-outline-secondary" asp-action="Index">Volver a matrículas</a>
    </div>
</div>

<h2 class="h4">Notas</h2>
@if (Model.Grades.Count == 0)
{
    <div class="alert alert-info">Esta matrícula todavía no tiene notas.</div>
}
else
{
    <div class="table-responsive border rounded mb-4">
        <table class="table align-middle mb-0">
            <thead>
                <tr>
                    <th>Tipo</th>
                    <th>Nota</th>
                    <th>Calificación</th>
                    <th>Fecha</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                @foreach (var grade in Model.Grades.OrderBy(g => g.GradedAt))
                {
                    <tr>
                        <td>@Html.DisplayFor(m => grade.GradeType)</td>
                        <td>@grade.Value</td>
                        <td>@Html.DisplayFor(m => grade.GradeEnum)</td>
                        <td>@grade.GradedAt?.ToString("dd/MM/yyyy")</td>
                        <td>
                            <a class="btn btn-sm btn-outline-secondary" asp-controller="Grades" asp-action="Details"
                               asp-route-id="@grade.Id">Ver</a>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
}

<h2 class="h4">Asistencias</h2>
@if (Model.Attendances.Count == 0)
{
    <div class="alert alert-info">Esta matrícula todavía no tiene asistencias apuntadas.</div>
}
else
{
    <div class="table-responsive border rounded">
        <table class="table align-middle mb-0">
            <thead>
                <tr>
                    <th>Lección</th>
                    <th>Fecha</th>
                    <th>Asistencia</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                @foreach (var attendance in Model.Attendances.OrderBy(a => a.Lesson!.StartDate))
                {
                    <tr>
                        <td>
                            @if (attendance.Lesson is not null)
                            {
                                <a asp-controller="Lessons" asp-action="Details"
                                   asp-route-id="@attendance.Lesson.Id">@attendance.Lesson.Name</a>
                            }
                        </td>
                        <td>@attendance.Lesson?.StartDate.ToString("dd/MM/yyyy HH:mm")</td>
                        <td>@Html.DisplayFor(m => attendance.Status)</td>
                        <td>
                            <a class="btn btn-sm btn-outline-secondary" asp-controller="Attendances" asp-action="Details"
                               asp-route-id="@attendance.Id">Ver</a>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
}

---

@model List<Enrollment>
@{
    ViewData["Title"] = "Mis matrículas";
}

<div class="d-flex justify-content-between align-items-center mb-3">
    <h1 class="mb-0">Mis matrículas</h1>
    @if (User.IsInRole(RoleNames.Admin))
    {
        <a class="btn btn-success" asp-action="Create">Nueva matrícula</a>
    }
</div>

<p>Hay @Model.Count matrículas.</p>

<table class="table table-striped">
    <thead>
        <tr>
            @if (User.IsInRole(RoleNames.Admin))
            {
                <th>Alumno</th>
            }
            <th>Curso</th>
            <th>Fecha Matriculación</th>
            <th>Estatus matrícula</th>
            <th></th>
        </tr>
    </thead>
    <tbody>
        @foreach (var enrollment in Model)
        {
            <tr>
                @if (User.IsInRole(RoleNames.Admin))
                {
                    <td>@(enrollment.Student?.DisplayName ?? enrollment.Student?.UserName ?? "Anónimo")</td>
                }
                <td>@enrollment.Course?.Name</td>
                <td>@enrollment.EnrolledAt</td>
                <td>@Html.DisplayFor(m => enrollment.Status)</td>
                <td>
                    <a class="btn btn-sm btn-outline-secondary" asp-action="Details" asp-route-id="@enrollment.Id">Ver</a>
                    @if (User.IsInRole(RoleNames.Admin))
                    {
                        <a class="btn btn-sm btn-outline-primary" asp-action="Edit" asp-route-id="@enrollment.Id">Editar</a>
                    }
                    @if (User.IsInRole(RoleNames.Admin))
                    {
                        <form asp-action="Delete" asp-route-id="@enrollment.Id" method="post" class="d-inline"
                              onsubmit="return confirm('¿Borrar esta matrícula?');">
                            <button class="btn btn-sm btn-outline-danger" type="submit">Borrar</button>
                        </form>
                    }
                </td>
            </tr>
        }
    </tbody>
</table>

---

@using G2AspNet.Models
@model Course
@{
    ViewData["Title"] = "Detalle de curso";
    var occupied = (int)ViewBag.OccupiedPlaces;
    var freePlaces = Model.MaxStudents - occupied;
    var myEnrollment = ViewBag.MyEnrollment as Enrollment;
    var canManage = (bool)ViewBag.CanManage;
}
<div class="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3">
    <h1 class="h2 mb-0">Detalle de curso</h1>
    <div class="d-flex gap-2">
        <form asp-action="Delete" asp-route-id="@Model.Id" method="post" class="d-inline"
              onsubmit="return confirm('¿Estás seguro de que deseas eliminar este curso?');">
            <button type="submit" class="btn btn-outline-danger">Eliminar</button>
        </form>
        <a class="btn btn-primary" asp-action="Edit" asp-route-id="@Model.Id">Editar</a>
    </div>
</div>

@if (User.Identity?.IsAuthenticated != true)
{
    <div class="alert alert-info">
        <a asp-controller="Account" asp-action="Login">Inicia sesión</a> para matricularte en este curso.
    </div>
}
else if (User.IsInRole(RoleNames.Student))
{
    if (myEnrollment != null)
    {
        <div class="alert alert-success">
            Ya estás matriculado en este curso.
            <a asp-controller="Enrollments" asp-action="Details" asp-route-id="@myEnrollment.Id">Ver mi matrícula</a>
        </div>
    }
    else if (Model.Status != CourseStatus.Open)
    {
        <div class="alert alert-secondary">Las matrículas de este curso están cerradas.</div>
    }
    else if (freePlaces <= 0)
    {
        <div class="alert alert-warning">Curso completo: no quedan plazas.</div>
    }
    else
    {
        <form asp-controller="Enrollments" asp-action="Enroll" method="post" class="mb-3">
            <input type="hidden" name="courseId" value="@Model.Id" />
            <button class="btn btn-success" type="submit">Matricularme</button>
        </form>
    }
}

@if (canManage && Model.Status == CourseStatus.Open)
{
    <form asp-action="Start" asp-route-id="@Model.Id" method="post" class="mb-3"
          onsubmit="return confirm('¿Iniciar el curso? Las matrículas sin pagar se darán de baja.');">
        <button class="btn btn-success" type="submit">Iniciar curso</button>
    </form>
}
else if (canManage && Model.Status == CourseStatus.InProgress)
{
    <form asp-action="Finish" asp-route-id="@Model.Id" method="post" class="mb-3"
          onsubmit="return confirm('¿Finalizar el curso? Las matrículas activas pasarán a completadas.');">
        <button class="btn btn-primary" type="submit">Finalizar curso</button>
    </form>
}

<div class="card">
    <div class="card-body p-4">
        <div class="d-flex align-items-center gap-3 mb-4">
            <div>
                <h2 class="h4 mb-0">@Model.Name</h2><span class="text-secondary">@Model.Description</span>
            </div>
        </div>
        <dl class="row mb-0">
            <dt class="col-sm-3">Nivel</dt>
            <dd class="col-sm-9">@Model.Level</dd>
            <dt class="col-sm-3">Estado</dt>
            <dd class="col-sm-9">@(Model.Status)</dd>
            <dt class="col-sm-3">Precio</dt>
            <dd class="col-sm-9">@Model.Price.ToString("C", System.Globalization.CultureInfo.GetCultureInfo("es-ES"))
            </dd>
            <dt class="col-sm-3">Fecha de inicio</dt>
            <dd class="col-sm-9">@Model.StartDate.ToString("dd/MM/yyyy")</dd>
            <dt class="col-sm-3">Fecha de finalización</dt>
            <dd class="col-sm-9">@Model.EndDate.ToString("dd/MM/yyyy")</dd>
            <dt class="col-sm-3">Plazas máximas</dt>
            <dd class="col-sm-9">@Model.MaxStudents</dd>
            <dt class="col-sm-3">Plazas libres</dt>
            <dd class="col-sm-9">@freePlaces de @Model.MaxStudents</dd>
            <dt class="col-sm-3">Centro de estudios</dt>
            <dd class="col-sm-9">@Model.Academy?.Name</dd>
            <dt class="col-sm-3">Profesor</dt>
            <dd class="col-sm-9">@Model.Teacher?.UserName</dd>
        </dl>
    </div>
    <div class="card-footer bg-transparent"><a class="btn btn-outline-secondary" asp-action="Index">Volver a cursos</a>
    </div>
</div>

@if (canManage)
{
    var courseEnrollments = (List<Enrollment>)ViewBag.CourseEnrollments;

    <h2 class="h4 mt-4">Matrículas del curso</h2>
    @if (courseEnrollments.Count == 0)
    {
        <div class="alert alert-info">Este curso todavía no tiene matrículas.</div>
    }
    else
    {
        <div class="table-responsive border rounded">
            <table class="table align-middle mb-0">
                <thead>
                    <tr><th>Alumno</th>
                    <th>Fecha</th>
                    <th>Pagada</th>
                    <th>Estado</th></tr>
                </thead>
                <tbody>
                    @foreach (var enrollment in courseEnrollments)
                    {
                        <tr>
                            <td>@(enrollment.Student?.DisplayName ?? enrollment.Student?.UserName)</td>
                            <td>@enrollment.EnrolledAt.ToString("dd/MM/yyyy")</td>
                            <td>@(enrollment.IsPaid ? "Sí" : "No")</td>
                            <td>@Html.DisplayFor(m => enrollment.Status)</td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    }
}






