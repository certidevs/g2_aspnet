using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;

namespace G2AspNet.Controllers;

public class AcademyController(ApplicationDbContext context) : Controller
{
    private readonly ApplicationDbContext context = context;

    public async Task<IActionResult> Index(string? search, string? city)
    {
        var query = context.Academies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a => a.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase));
        }

        if (city != null) query = query.Where(a => a.City == city);

        ViewBag.Search = search;
        ViewBag.City = city;

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
