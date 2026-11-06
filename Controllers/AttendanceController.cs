using Microsoft.AspNetCore.Mvc;
using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.ViewModels.Attendances;
using Microsoft.EntityFrameworkCore;

namespace G2AspNet.Controllers;

public class AttendanceController : Controller
{
    private readonly ApplicationDbContext context;

    public AttendanceController(ApplicationDbContext context)
    {
        this.context = context;
    }

    public IActionResult Index(string? search)
    {
        var query = context.Attendances.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(attendance => attendance.Status.ToString().Contains(search));
        }

        var model = new AttendanceIndexViewModel
        {
            Search = search,
            Attendances = query
                .OrderBy(attendance => attendance.Id)
                .Select(attendance => new AttendanceListItemViewModel { Attendance = attendance })
                .ToList()
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create(int lessonId, int enrollmentId)
    {
        var attendance = new Attendance
        {
            LessonId = lessonId,
            EnrollmentId = enrollmentId,
            Status = AttendanceStatus.Present
        };

        return View("Form", attendance);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Attendance attendance)
    {
        if (ModelState.IsValid)
        {
            context.Attendances.Add(attendance);
            context.SaveChanges();
            return RedirectToAction("Details", "Lessons", new { id = attendance.LessonId });
        }

        return View("Form", attendance);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Save(Attendance attendance){
        if (ModelState.IsValid)
        {
            context.Attendances.Update(attendance);
            context.SaveChanges();
            return RedirectToAction("Details", "Lessons", new { id = attendance.LessonId });
        }

        return View("Form", attendance);
    }

}