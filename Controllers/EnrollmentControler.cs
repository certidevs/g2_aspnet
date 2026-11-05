using Microsoft.AspNetCore.Mvc;
using G2AspNet.Data;
using G2AspNet.Models;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace G2AspNet.Controllers;

public class EnrollmentController : Controller
{
    private readonly ApplicationDbContext context;

    public EnrollmentController(ApplicationDbContext context)
    {
        this.context = context;
    }

    [Authorize]
    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var enrollments = context.Enrollments
            .Include(e => e.Course)
            .Where(e => e.StudentId == userId)
            .OrderByDescending(e => e.EnrolledAt)
            .ToList();

        return View(enrollments);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Form", new Enrollment());
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var enrollment = context.Enrollments.Find(id);
        if (enrollment == null)
        {
            return NotFound();
        }

        return View("Form", enrollment);
    }

    [HttpPost]
    public IActionResult Save(Enrollment enrollment)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", enrollment);
        }

        if (enrollment.Id == 0)
        {
            context.Enrollments.Add(enrollment);
        }
        else
        {
            context.Enrollments.Update(enrollment);
        }

        context.SaveChanges();

        TempData["Message"] = "Matriculación enviada correctamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        var enrollment = context.Enrollments.Find(id);
        if (enrollment == null)
        {
            return NotFound();
        }

        context.Enrollments.Remove(enrollment);
        context.SaveChanges();

        TempData["Message"] = "Matriculación eliminada.";
        return RedirectToAction("Index");
    }
}