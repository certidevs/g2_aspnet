using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList (desplegables del formulario)
using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.ViewModels.Courses;
using Microsoft.EntityFrameworkCore;

namespace G2AspNet.Controllers;

public class CoursesController : Controller
{
    private readonly ApplicationDbContext context;

    public CoursesController(ApplicationDbContext context)
    {
        this.context = context;
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

        return View("Details", course);
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
