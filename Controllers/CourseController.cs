using Microsoft.AspNetCore.Mvc;
using G2AspNet.Data;
using G2AspNet.Models;

namespace G2AspNet.Controllers;

public class CoursesController : Controller
{
    private readonly ApplicationDbContext context;

    public CoursesController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public IActionResult Index()
    {
        var courses = context.Courses.OrderBy(c => c.Name).ToList();
        return View(courses);
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

    [HttpPost]
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