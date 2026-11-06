using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;
using G2AspNet.Models;

namespace G2AspNet.Controllers;

public class ClassroomController(ApplicationDbContext context) : Controller
{

    public async Task<IActionResult> Index()
    {
        var classroom = await context.Classrooms
        .Include(c => c.Academy)
        .OrderBy(c => c.Name)
        .ToListAsync();

        return View(classroom);
    }
    public async Task<IActionResult> Details(int id)
    {
        var classroom = await context.Classrooms
        .Include(c => c.Academy)
        .Include(c => c.Lessons)
        .FirstOrDefaultAsync(a => a.Id == id);

        if (classroom == null)
        {
            return NotFound();
        }

        return View(classroom);
    }
}
