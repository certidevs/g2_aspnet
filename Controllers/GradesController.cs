using Microsoft.AspNetCore.Mvc;
using G2AspNet.Data;
using G2AspNet.Models;

namespace G2AspNet.Controllers;

public class GradesController : Controller
{
    private readonly ApplicationDbContext context;

    public GradesController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public IActionResult Index()
    {
        var grades = context.Grades.OrderBy(g => g.NameStudent).ToList();
        return View(grades);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Form", new Grade());
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var grade = context.Grades.Find(id);
        if (grade == null)
        {
            return NotFound();
        }

        return View("Form", grade);
    }

    [HttpPost]
    public IActionResult Save(Grade grade)
    {
        if (!ModelState.IsValid)
        {
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