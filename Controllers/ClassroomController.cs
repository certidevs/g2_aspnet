using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;

namespace G2AspNet.Controllers;

public class ClassroomController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext context = context;

    public IActionResult Index()
    {
        var classrooms = context.Classrooms.Include(c => c.Name).OrderBy(c => c.Name).ToList();
        return View(classrooms);
    }
/*     public IActionResult Details(int id)
    {
        var academy = context.Academies.Include(a => a.Courses).FirstOrDefault(a => a.Id == id);
        if (academy == null)
        {
            return NotFound();
        }

        return View(academy);
    } */
}