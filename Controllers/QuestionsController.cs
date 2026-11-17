using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.Utilities;

namespace G2AspNet.Controllers;

public class QuestionsController : Controller
{
    private readonly ApplicationDbContext context;

    public QuestionsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    // Formulario para crear una pregunta dentro de un examen.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Create(int examId)
    {
        var exam = context.Exams.Find(examId);
        if (exam == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && exam.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        var question = new Question { ExamId = examId };
        return View("Form", question);
    }

    // Formulario para editar una pregunta existente.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var question = context.Questions.Include(q => q.Exam).FirstOrDefault(q => q.Id == id);
        if (question == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && question.Exam?.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        return View("Form", question);
    }

    // Guarda una pregunta (crea o actualiza según Id).
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Save(Question question)
    {
        var exam = context.Exams.Find(question.ExamId);
        if (exam == null)
        {
            ModelState.AddModelError(nameof(Question.ExamId), "El examen no existe.");
        }

        if (!ModelState.IsValid)
        {
            return View("Form", question);
        }

        if (question.Id == 0)
        {
            context.Questions.Add(question);
        }
        else
        {
            var existing = context.Questions.Find(question.Id);
            if (existing == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole(RoleNames.Admin);
            if (!isAdmin && existing.Exam?.TeacherId != User.GetRequiredUserId())
            {
                return Forbid();
            }

            existing.Text = question.Text;
            existing.Option1 = question.Option1;
            existing.Option2 = question.Option2;
            existing.Option3 = question.Option3;
            existing.Option4 = question.Option4;
            existing.CorrectOption = question.CorrectOption;
        }

        context.SaveChanges();

        TempData["Message"] = "Pregunta guardada correctamente.";
        return RedirectToAction("Details", "Exams", new { id = question.ExamId });
    }

    // Elimina una pregunta de un examen.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var question = context.Questions.Include(q => q.Exam).FirstOrDefault(q => q.Id == id);
        if (question == null)
        {
            return NotFound();
        }

        var examId = question.ExamId;
        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && question.Exam?.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        context.Questions.Remove(question);
        context.SaveChanges();

        TempData["Message"] = "Pregunta eliminada correctamente.";
        return RedirectToAction("Details", "Exams", new { id = examId });
    }
}
