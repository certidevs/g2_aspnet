using Microsoft.AspNetCore.Mvc;
using G2AspNet.Data;
using G2AspNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Utilities;

namespace G2AspNet.Controllers;

public class GradesController : Controller
{
    private readonly ApplicationDbContext context;

    public GradesController(ApplicationDbContext context)
    {
        this.context = context;
    }

    [Authorize]
    public IActionResult Index()
    {
        var query = context.Grades
            .Include(g => g.Enrollment).ThenInclude(e => e!.Student)
            .Include(g => g.Enrollment).ThenInclude(e => e!.Course)
            .AsQueryable();

        var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher);
        if (!isStaff)
        {
            var userId = User.GetRequiredUserId();
            query = query.Where(g => g.Enrollment!.StudentId == userId);
        }

        var grades = query.OrderBy(g => g.EnrollmentId).ToList();
        return View(grades);
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Create()
    {
        return View("Form", new Grade());
    }

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
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

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
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

    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
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