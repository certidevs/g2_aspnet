using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public IActionResult Create()
    {
        return View("Form", new Course());
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var course = context.Courses.Find(id);
        if (course == null)
        {
            return NotFound();
        }

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
        if (!ModelState.IsValid)
        {
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

        context.Courses.Remove(course);
        context.SaveChanges();

        TempData["Message"] = "Curso borrado correctamente.";
        return RedirectToAction("Index");
    }
}
