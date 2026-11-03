using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;

namespace G2AspNet.Controllers;

public class AcademyController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext context = context;

    public async Task<IActionResult> Index(string? search, string? location)
    {
        var query = context.Academies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(a => a.Name.ToLower().Contains(search, StringComparison.CurrentCultureIgnoreCase));

        if (location != null) query = query.Where(a => a.Location == location);

        ViewBag.Search = search;
        ViewBag.Location = location;

        var academies = await query.OrderBy(a => a.Name).ToListAsync();
        return View(academies);
    }
    public async Task<IActionResult> Details(int id)
    {
        var academy = await context.Academies
        .Include(a => a.Courses)
        .Include(a => a.Classrooms)
        .FirstOrDefaultAsync(a => a.Id == id);
        
        if (academy == null)
        {
            return NotFound();
        }

        return View(academy);
    }
}
