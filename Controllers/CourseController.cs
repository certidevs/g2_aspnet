using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;

namespace G2AspNet.Controllers;

public class CourseController : Controller
{
    private readonly ApplicationDbContext context;

    public CourseController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public IActionResult Index()
    {
        var course = context.Courses.OrderBy(a => a.Name).ToList();
        return View(course);
    }
    /* public IActionResult Details(int id)
    {
        var course = context.Courses.Include(a => a.Lessons).FirstOrDefault(a => a.Id == id);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    } */
}
