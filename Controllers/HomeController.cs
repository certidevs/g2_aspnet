using G2AspNet.Data;
using G2AspNet.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace G2AspNet.Controllers;

/// <summary>Página de inicio: cifras de la plataforma y próximos cursos abiertos.</summary>
public sealed class HomeController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewData["Academies"] = await context.Academies.CountAsync();
        ViewData["Courses"] = await context.Courses.CountAsync(c => c.Status == CourseStatus.Open || c.Status == CourseStatus.InProgress);
        ViewData["Enrollments"] = await context.Enrollments.CountAsync();
        ViewData["Lessons"] = await context.Lessons.CountAsync();

        // Los tres cursos abiertos que empiezan antes.
        var upcoming = await context.Courses
            .AsNoTracking()
            .Include(c => c.Academy)
            .Where(c => c.Status == CourseStatus.Open)
            .OrderBy(c => c.StartDate)
            .Take(3)
            .ToListAsync();
        ViewData["UpcomingCourses"] = upcoming;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });
}
