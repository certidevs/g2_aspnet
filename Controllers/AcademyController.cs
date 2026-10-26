using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;

namespace G2AspNet.Controllers;

public class AcademyController : Controller
{
    private readonly ApplicationDbContext context;

    public AcademyController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public IActionResult Index()
    {
        var academies = context.Academies.Include(a => a.Courses).OrderBy(a => a.Name).ToList();
        return View(academies);
    }
    public IActionResult Details(int id)
    {
        var academy = context.Academies.Include(a => a.Courses).FirstOrDefault(a => a.Id == id);
        if (academy == null)
        {
            return NotFound();
        }

        return View(academy);
    }
}
