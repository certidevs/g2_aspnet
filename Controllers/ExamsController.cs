using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;           // Serializar es transformar de objeto a texto (JSON)
using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.Utilities;

namespace G2AspNet.Controllers;

public class ExamsController : Controller
{
    private readonly ApplicationDbContext context;

    public ExamsController(ApplicationDbContext context)
    {
        this.context = context;
    }

    // DTO Data Transfer Object, son datos planos.
    // DTOs planos para serializar en TempData sin ciclos de navegación (Exam <-> Question). Exam y Question tienen referencias mutuas y la serializacion entra en bucle por ello.
    private sealed class ExamBorradorDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CourseId { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    private sealed class QuestionBorradorDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Option1 { get; set; } = string.Empty;
        public string Option2 { get; set; } = string.Empty;
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        public int CorrectOption { get; set; }
        public int ExamId { get; set; }
    }

    // Detecta si el formulario de pregunta tiene algún campo relleno pero no guardado.
    private bool AreQuestionNotSaved(Question pregunta)
    {
        if (pregunta == null) return false;
        return !string.IsNullOrWhiteSpace(pregunta.Text)
            || !string.IsNullOrWhiteSpace(pregunta.Option1)
            || !string.IsNullOrWhiteSpace(pregunta.Option2)
            || !string.IsNullOrWhiteSpace(pregunta.Option3)
            || !string.IsNullOrWhiteSpace(pregunta.Option4);
    }

    // Mapea un ExamBorradorDto a una entidad Exam.
    private Exam MapExamDtoToEntity(ExamBorradorDto dto)
    {
        return new Exam
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            CourseId = dto.CourseId,
            TeacherId = dto.TeacherId,
            IsActive = dto.IsActive,
            CreatedAt = dto.CreatedAt,
            Questions = new List<Question>()
        };
    }

    // Mapea un QuestionBorradorDto a una entidad Question.
    private Question MapQuestionDtoToEntity(QuestionBorradorDto dto)
    {
        return new Question
        {
            Id = dto.Id,
            Text = dto.Text,
            Option1 = dto.Option1,
            Option2 = dto.Option2,
            Option3 = dto.Option3,
            Option4 = dto.Option4,
            CorrectOption = dto.CorrectOption,
            ExamId = dto.ExamId,
            Exam = null
        };
    }

    // Listado de exámenes.
    // Un admin o profesor ve todos; un alumno solo ve los exámenes activos.
    [Authorize]
    public IActionResult Index()
    {
        var query = context.Exams
            .Include(e => e.Course)
            .Include(e => e.Teacher)
            .AsQueryable();

        if (!User.IsInRole(RoleNames.Admin) && !User.IsInRole(RoleNames.Teacher))
        {
            query = query.Where(e => e.IsActive);
        }

        var exams = query.OrderByDescending(e => e.CreatedAt).ToList();
        return View(exams);
    }

    // Detalle de un examen con sus preguntas.
    [Authorize]
    [HttpGet]
    public IActionResult Details(int id)
    {
        var exam = context.Exams
            .Include(e => e.Course)
            .Include(e => e.Teacher)
            .Include(e => e.Questions)
            .FirstOrDefault(e => e.Id == id);

        if (exam == null)
        {
            return NotFound();
        }

        var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher);
        if (!isStaff && exam.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        return View(exam);
    }



    // Vista del examen para el alumno: responder preguntas.
    // Solo puede acceder si es staff o si tiene una matrícula válida (Pending/Active) en el curso del examen,
    // el examen está dentro de sus fechas de apertura/cierre y aún le quedan intentos.
    [Authorize]
    [HttpGet]
    public IActionResult Take(int id)
    {
        var exam = context.Exams
            .Include(e => e.Course)
            .Include(e => e.Teacher)
            .Include(e => e.Questions)
            .FirstOrDefault(e => e.Id == id);

        if (exam == null)
        {
            return NotFound();
        }

        var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher);
        var userId = User.GetRequiredUserId();

        if (!isStaff)
        {
            // Verificar matrícula válida
            var tieneMatricula = context.Enrollments
                .Any(e => e.CourseId == exam.CourseId
                    && e.StudentId == userId
                    && EnrollmentRules.PlaceTakingStatuses.Contains(e.Status));

            if (!tieneMatricula)
            {
                return Forbid();
            }

            // Verificar fechas de apertura y cierre
            var ahora = DateTime.UtcNow;
            if (exam.FechaApertura.HasValue && ahora < exam.FechaApertura.Value)
            {
                TempData["Error"] = $"Este examen aún no está disponible. Se abre el {exam.FechaApertura.Value:dd/MM/yyyy HH:mm}.";
                return RedirectToAction("Index");
            }
            if (exam.FechaCierre.HasValue && ahora > exam.FechaCierre.Value)
            {
                TempData["Error"] = $"Este examen ya se cerró el {exam.FechaCierre.Value:dd/MM/yyyy HH:mm}.";
                return RedirectToAction("Index");
            }

            // Verificar intentos restantes
            var intentosRealizados = context.ExamAttempts
                .Count(e => e.ExamId == id && e.StudentId == userId && e.IsCompleted);

            if (intentosRealizados >= exam.MaxIntentos)
            {
                TempData["Error"] = $"Has agotado los {exam.MaxIntentos} intentos permitidos para este examen.";
                return RedirectToAction("Index");
            }

            ViewBag.IntentosRestantes = exam.MaxIntentos - intentosRealizados;
            ViewBag.MaxIntentos = exam.MaxIntentos;
        }
        else
        {
            ViewBag.IntentosRestantes = null;
            ViewBag.MaxIntentos = null;
        }

        return View("StudentTake", exam);
    }

    // Formulario para crear un nuevo examen (con borrador de preguntas acumuladas).
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Create()
    {
        var examJson = TempData.Peek("ExamBorrador") as string;
        var preguntasJson = TempData.Peek("PreguntasBorrador") as string;

        Exam exam;
        List<Question> preguntas;

        if (!string.IsNullOrEmpty(examJson) && !string.IsNullOrEmpty(preguntasJson))
        {
            var examenDto = JsonSerializer.Deserialize<ExamBorradorDto>(examJson);
            var preguntasDtos = JsonSerializer.Deserialize<List<QuestionBorradorDto>>(preguntasJson) ?? new List<QuestionBorradorDto>();

            exam = MapExamDtoToEntity(examenDto ?? new ExamBorradorDto());
            preguntas = preguntasDtos.Select(MapQuestionDtoToEntity).ToList();
        }
        else
        {
            exam = new Exam();
            preguntas = new List<Question>();
        }

        ViewBag.Preguntas = preguntas;
        ViewBag.PreguntaActual = new Question { ExamId = exam.Id };
        ViewBag.EsPrimeraPregunta = preguntas.Count == 0;

        ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
        ViewBag.AreQuestionNotSaved = AreQuestionNotSaved((Question)ViewBag.PreguntaActual!);
        return View("Form", exam);
    }

    // Cancela la creación del examen y limpia el borrador de TempData.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    public IActionResult Cancelar(int? id)
    {
        TempData.Remove("ExamBorrador");
        TempData.Remove("PreguntasBorrador");
        if (id.HasValue && id.Value > 0)
        {
            return RedirectToAction("Details", new { id = id.Value });
        }
        
        return RedirectToAction("Index");
    }

    // Formulario para editar un examen existente.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Edit(int id)
    {
        var exam = context.Exams.Include(e => e.Questions).FirstOrDefault(e => e.Id == id);
        if (exam == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && exam.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
        ViewBag.Preguntas = exam.Questions ?? new List<Question>();
        ViewBag.PreguntaActual = new Question { ExamId = exam.Id };
        ViewBag.EsPrimeraPregunta = (exam.Questions ?? new List<Question>()).Count == 0;
        ViewBag.AreQuestionNotSaved = AreQuestionNotSaved((Question)ViewBag.PreguntaActual!);

        // Guardar los datos planos del examen y las preguntas en TempData (sin ciclos de navegación).
        var examenDto = new ExamBorradorDto
        {
            Id = exam.Id,
            Title = exam.Title,
            Description = exam.Description,
            CourseId = exam.CourseId,
            TeacherId = exam.TeacherId,
            IsActive = exam.IsActive,
            CreatedAt = exam.CreatedAt
        };
        TempData["ExamBorrador"] = JsonSerializer.Serialize(examenDto);

        var preguntasDtos = (exam.Questions ?? new List<Question>()).Select(q => new QuestionBorradorDto
        {
            Id = q.Id,
            Text = q.Text,
            Option1 = q.Option1,
            Option2 = q.Option2,
            Option3 = q.Option3,
            Option4 = q.Option4,
            CorrectOption = q.CorrectOption,
            ExamId = q.ExamId
        }).ToList();
        TempData["PreguntasBorrador"] = JsonSerializer.Serialize(preguntasDtos);

        return View("Form", exam);
    }

    // Guarda una pregunta en el borrador, cancela una pregunta, o guarda el examen completo.
    // accion="saveQuestion": acumula la pregunta actual en el borrador.
    // accion="saveExam": valida y guarda el examen con todas sus preguntas en la base de datos.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Save(Exam exam, Question pregunta, string accion)
    {
        if (accion == "saveQuestion")
        {
            // Guardar datos del examen tal como están (planos, sin ciclos)
            var examenDto = new ExamBorradorDto
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                CourseId = exam.CourseId,
                TeacherId = exam.TeacherId,
                IsActive = exam.IsActive,
                CreatedAt = exam.CreatedAt
            };
            TempData["ExamBorrador"] = JsonSerializer.Serialize(examenDto);

            // Leer preguntas existentes del borrador (sin marcar para borrar)
            var preguntasJson = TempData.Peek("PreguntasBorrador") as string;
            var preguntasDtos = string.IsNullOrEmpty(preguntasJson)
                ? new List<QuestionBorradorDto>()
                : JsonSerializer.Deserialize<List<QuestionBorradorDto>>(preguntasJson) ?? new List<QuestionBorradorDto>();

            // Agregar la pregunta actual si tiene texto
            if (pregunta != null && !string.IsNullOrWhiteSpace(pregunta.Text))
            {
                preguntasDtos.Add(new QuestionBorradorDto
                {
                    Id = 0,
                    Text = pregunta.Text,
                    Option1 = pregunta.Option1,
                    Option2 = pregunta.Option2,
                    Option3 = pregunta.Option3,
                    Option4 = pregunta.Option4,
                    CorrectOption = pregunta.CorrectOption,
                    ExamId = pregunta.ExamId
                });
            }

            TempData["PreguntasBorrador"] = JsonSerializer.Serialize(preguntasDtos);

            return RedirectToAction("Create");
        }

       /*  if (accion == "addQuestion")
        {
            // Mantener los datos del examen, descartar pregunta actual
            var examenDto = new ExamBorradorDto
            {
                Id = exam.Id,
                Title = exam.Title,
                Description = exam.Description,
                CourseId = exam.CourseId,
                TeacherId = exam.TeacherId,
                IsActive = exam.IsActive,
                CreatedAt = exam.CreatedAt
            };
            TempData["ExamBorrador"] = JsonSerializer.Serialize(examenDto);
            return RedirectToAction("Create");
        } */

        // accion == "saveExam"
        ModelState.Remove(nameof(Exam.TeacherId));

        // Limpiar errores del modelo de pregunta para que no bloqueen el guardado del examen
        var keysToRemove = ModelState.Keys.Where(k => k.StartsWith("pregunta.")).ToList();
        foreach (var key in keysToRemove)
        {
            ModelState.Remove(key);
        }

        if (!ModelState.IsValid)
        {
            var preguntasJson = TempData.Peek("PreguntasBorrador") as string;
            var preguntasDtos = string.IsNullOrEmpty(preguntasJson)
                ? new List<QuestionBorradorDto>()
                : JsonSerializer.Deserialize<List<QuestionBorradorDto>>(preguntasJson) ?? new List<QuestionBorradorDto>();

            var preguntas = preguntasDtos.Select(MapQuestionDtoToEntity).ToList();

            ViewBag.Preguntas = preguntas;
            ViewBag.PreguntaActual = pregunta ?? new Question { ExamId = exam.Id };
            ViewBag.EsPrimeraPregunta = preguntas.Count == 0;
            ViewBag.AreQuestionNotSaved = AreQuestionNotSaved(pregunta!);
            ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
            return View("Form", exam);
        }

        // Verificar que hay al menos una pregunta guardada
        var preguntasBorradorJson = TempData.Peek("PreguntasBorrador") as string;
        var preguntasDtosFinales = string.IsNullOrEmpty(preguntasBorradorJson)
            ? new List<QuestionBorradorDto>()
            : JsonSerializer.Deserialize<List<QuestionBorradorDto>>(preguntasBorradorJson) ?? new List<QuestionBorradorDto>();

        // Si estamos editando un examen existente y el borrador está vacío,
        // cargar las preguntas desde la base de datos para que se reconozcan como guardadas.
        if (preguntasDtosFinales.Count == 0 && exam.Id > 0)
        {
            var preguntasDb = context.Questions
                .Where(q => q.ExamId == exam.Id)
                .OrderBy(q => q.Id)
                .ToList();
            if (preguntasDb.Count > 0)
            {
                preguntasDtosFinales = preguntasDb.Select(q => new QuestionBorradorDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Option1 = q.Option1,
                    Option2 = q.Option2,
                    Option3 = q.Option3,
                    Option4 = q.Option4,
                    CorrectOption = q.CorrectOption,
                    ExamId = q.ExamId
                }).ToList();
                TempData["PreguntasBorrador"] = JsonSerializer.Serialize(preguntasDtosFinales);
            }
        }

        if (preguntasDtosFinales.Count == 0)
        {
            ModelState.AddModelError("", "El examen debe tener al menos una pregunta guardada.");
            ViewBag.Preguntas = new List<Question>();
            ViewBag.PreguntaActual = new Question { ExamId = exam.Id };
            ViewBag.EsPrimeraPregunta = true;
            ViewBag.AreQuestionNotSaved = AreQuestionNotSaved(new Question { ExamId = exam.Id });
            ViewBag.Courses = new SelectList(context.Courses.OrderBy(c => c.Name).ToList(), "Id", "Name");
            return View("Form", exam);
        }

        if (exam.Id == 0)
        {
            exam.TeacherId = User.GetRequiredUserId();
            exam.CreatedAt = DateTime.UtcNow;
            context.Exams.Add(exam);
            context.SaveChanges(); // Para obtener el Id generado
        }
        else
        {
            var existing = context.Exams.FirstOrDefault(e => e.Id == exam.Id);
            if (existing == null)
            {
                return NotFound();
            }

            var isAdmin = User.IsInRole(RoleNames.Admin);
            if (!isAdmin && existing.TeacherId != User.GetRequiredUserId())
            {
                return Forbid();
            }

            existing.Title = exam.Title;
            existing.Description = exam.Description;
            existing.CourseId = exam.CourseId;
            existing.IsActive = exam.IsActive;

            exam = existing;
        }

        // Guardar preguntas del borrador: Add si es nueva (Id==0), Update si ya existe (Id>0)
        var idsBorrador = preguntasDtosFinales.Where(p => p.Id > 0).Select(p => p.Id).ToList();

        foreach (var dto in preguntasDtosFinales)
        {
            var entidad = MapQuestionDtoToEntity(dto);
            entidad.ExamId = exam.Id;
            if (entidad.Id == 0)
            {
                context.Questions.Add(entidad);
            }
            else
            {
                context.Questions.Update(entidad);
            }
        }

        // Borrar preguntas de la base de datos que ya no están en el borrador
        if (exam.Id > 0)
        {
            var preguntasAborrar = context.Questions
                .AsNoTracking()
                .Where(q => q.ExamId == exam.Id && !idsBorrador.Contains(q.Id))
                .ToList();
            context.Questions.RemoveRange(preguntasAborrar);
        }

        context.SaveChanges();

        // Limpiar borrador de TempData
        TempData.Remove("ExamBorrador");
        TempData.Remove("PreguntasBorrador");

        TempData["Message"] = "Examen guardado correctamente.";
        return RedirectToAction("Details", new { id = exam.Id });
    }



    // Guarda el intento de examen de un alumno y calcula la nota sobre 10.
    // Recibe las respuestas marcadas en el formulario del alumno.
    [Authorize]
    [HttpPost]
    public IActionResult SaveAttempt(int examId)
    {
        var exam = context.Exams
            .Include(e => e.Questions)
            .FirstOrDefault(e => e.Id == examId);

        if (exam == null)
        {
            return NotFound();
        }

        var userId = User.GetRequiredUserId();
        var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher);

        if (!isStaff)
        {
            // Verificar matrícula válida
            var tieneMatricula = context.Enrollments
                .Any(e => e.CourseId == exam.CourseId
                    && e.StudentId == userId
                    && EnrollmentRules.PlaceTakingStatuses.Contains(e.Status));

            if (!tieneMatricula)
            {
                return Forbid();
            }

            // Verificar fechas
            var ahora = DateTime.UtcNow;
            if (exam.FechaApertura.HasValue && ahora < exam.FechaApertura.Value)
            {
                TempData["Error"] = $"Este examen aún no está disponible.";
                return RedirectToAction("Index");
            }
            if (exam.FechaCierre.HasValue && ahora > exam.FechaCierre.Value)
            {
                TempData["Error"] = $"Este examen ya se cerró.";
                return RedirectToAction("Index");
            }

            // Verificar intentos restantes
            var intentosRealizados = context.ExamAttempts
                .Count(e => e.ExamId == examId && e.StudentId == userId && e.IsCompleted);

            if (intentosRealizados >= exam.MaxIntentos)
            {
                TempData["Error"] = $"Has agotado los {exam.MaxIntentos} intentos permitidos.";
                return RedirectToAction("Index");
            }
        }

        var questions = exam.Questions.OrderBy(q => q.Id).ToList();
        var totalQuestions = questions.Count;
        var aciertos = 0;

        var attempt = new ExamAttempt
        {
            StudentId = userId,
            ExamId = examId,
            StartedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow,
            IsCompleted = true
        };

        context.ExamAttempts.Add(attempt);
        context.SaveChanges(); // Para obtener el Id del intento

        foreach (var question in questions)
        {
            var selectedKey = $"respuesta-{question.Id}";
            var selectedValue = Request.Form[selectedKey].FirstOrDefault();
            var comentarioKey = $"comentario-{question.Id}";
            var comentarioValue = Request.Form[comentarioKey].FirstOrDefault();

            int selectedOption = 0;
            if (!string.IsNullOrWhiteSpace(selectedValue) && int.TryParse(selectedValue, out int parsed))
            {
                selectedOption = parsed;
            }

            bool isCorrect = selectedOption == question.CorrectOption;
            if (isCorrect) aciertos++;

            var answer = new ExamAnswer
            {
                ExamAttemptId = attempt.Id,
                QuestionId = question.Id,
                SelectedOption = selectedOption,
                IsCorrect = isCorrect,
                ComentarioAlumno = string.IsNullOrWhiteSpace(comentarioValue) ? null : comentarioValue.Trim()
            };

            context.ExamAnswers.Add(answer);
        }

        // Calcular nota sobre 10
        double score = totalQuestions > 0
            ? Math.Round((double)aciertos / totalQuestions * 10, 2)
            : 0;

        attempt.Score = score;
        context.SaveChanges();

        TempData["Message"] = $"Examen completado. Nota: {score} / 10";
        return RedirectToAction("Result", new { attemptId = attempt.Id });
    }

    // Muestra el resultado de un intento de examen.
    [Authorize]
    [HttpGet]
    public IActionResult Result(int attemptId)
    {
        var attempt = context.ExamAttempts
            .Include(e => e.Exam).ThenInclude(e => e!.Questions)
            .Include(e => e.Answers).ThenInclude(a => a!.Question)
            .FirstOrDefault(e => e.Id == attemptId);

        if (attempt == null)
        {
            return NotFound();
        }

        var userId = User.GetRequiredUserId();
        var isStaff = User.IsInRole(RoleNames.Admin) || User.IsInRole(RoleNames.Teacher);

        if (!isStaff && attempt.StudentId != userId)
        {
            return Forbid();
        }

        return View(attempt);
    }

    // Vista para el profesor: ver los últimos intentos de cada alumno para un examen.
    // Solo accesible cuando el examen ha cerrado (FechaCierre ha pasado).
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult Validate(int id)
    {
        var exam = context.Exams
            .Include(e => e.Course)
            .FirstOrDefault(e => e.Id == id);

        if (exam == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && exam.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        // Solo accesible cuando el examen ha cerrado
        var ahora = DateTime.UtcNow;
        if (exam.FechaCierre.HasValue && ahora < exam.FechaCierre.Value)
        {
            TempData["Error"] = $"Este examen aún está abierto. Se cierra el {exam.FechaCierre.Value:dd/MM/yyyy HH:mm}.";
            return RedirectToAction("Details", new { id = exam.Id });
        }

        // Obtener el último intento de cada alumno para este examen
        var intentos = context.ExamAttempts
            .Include(ea => ea.Student)
            .Include(ea => ea.Answers).ThenInclude(a => a.Question)
            .Where(ea => ea.ExamId == id && ea.IsCompleted)
            .OrderByDescending(ea => ea.CompletedAt)
            .ToList();

        // Agrupar por alumno y quedarnos con el más reciente de cada uno
        var ultimosIntentos = intentos
            .GroupBy(ea => ea.StudentId)
            .Select(g => g.First())
            .OrderByDescending(ea => ea.CompletedAt)
            .ToList();

        ViewBag.Exam = exam;
        return View(ultimosIntentos);
    }

    // Vista detallada de un intento específico para el profesor (revisar respuestas y comentarios).
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpGet]
    public IActionResult TryDetail(int attemptId)
    {
        var attempt = context.ExamAttempts
            .Include(ea => ea.Student)
            .Include(ea => ea.Exam).ThenInclude(e => e!.Questions)
            .Include(ea => ea.Answers).ThenInclude(a => a!.Question)
            .FirstOrDefault(ea => ea.Id == attemptId);

        if (attempt == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && attempt.Exam?.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        return View(attempt);
    }

    // Guarda la nota y comentario del profesor para un intento de examen.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult TryDetail(int attemptId, double profesorNota, string comentarioProfesor)
    {
        var attempt = context.ExamAttempts
            .Include(ea => ea.Exam)
            .FirstOrDefault(ea => ea.Id == attemptId);

        if (attempt == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && attempt.Exam?.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        attempt.ProfesorNota = profesorNota;
        attempt.ComentarioProfesor = string.IsNullOrWhiteSpace(comentarioProfesor) ? null : comentarioProfesor.Trim();
        attempt.Validado = true;
        attempt.FechaValidacion = DateTime.UtcNow;

        context.SaveChanges();

        TempData["Message"] = "Intento validado correctamente.";
        return RedirectToAction("Validate", new { id = attempt.ExamId });
    }

    // Marca todos los intentos de un examen como validados.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult ValidateAll(int id)
    {
        var exam = context.Exams
            .FirstOrDefault(e => e.Id == id);

        if (exam == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && exam.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        var intentos = context.ExamAttempts
            .Where(ea => ea.ExamId == id && ea.IsCompleted && !ea.Validado)
            .ToList();

        foreach (var attempt in intentos)
        {
            attempt.Validado = true;
            attempt.FechaValidacion = DateTime.UtcNow;
        }

        context.SaveChanges();

        TempData["Message"] = $"Se han validado {intentos.Count} intentos.";
        return RedirectToAction("Validate", new { id = exam.Id });
    }

    // Elimina un examen y sus preguntas asociadas.
    [Authorize(Roles = RoleNames.Admin + "," + RoleNames.Teacher)]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var exam = context.Exams.Include(e => e.Questions).FirstOrDefault(e => e.Id == id);
        if (exam == null)
        {
            return NotFound();
        }

        var isAdmin = User.IsInRole(RoleNames.Admin);
        if (!isAdmin && exam.TeacherId != User.GetRequiredUserId())
        {
            return Forbid();
        }

        context.Exams.Remove(exam);
        context.SaveChanges();

        TempData["Message"] = "Examen eliminado correctamente.";
        return RedirectToAction("Details");
    }
}
