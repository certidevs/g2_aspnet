# GRUPO 2: Academias

1. Matricularse (alumno)
– ya hecho: EnrollmentsController.cs:111
    asigna `StudentId = User.GetRequiredUserId()`Es el mejor ejemplo para la clase.
Mejoras posibles:
    - un botón "Matricularme" en Courses/Details;
    - solo en cursos Open;
    - respetar MaxStudents;
    - no permitir matricularse dos veces.
2. Mis matrículas y mis notas
– ya hecho: Enrollments y Grades ya filtran por alumno.
3. Mis cursos (profesor):
Courses/Index filtrado por `TeacherId == userId`.
4. Pasar lista (profesor):
en Lessons/Details, un formulario con todos los matriculados del curso y un estado por alumno (Present, Absent, Late o Excused), que
    guarda todas las Attendance de una vez. Solo el profesor de ese curso.
5. Poner notas (profesor): solo en sus cursos.
6. Darse de baja (alumno): su matrícula pasa a Dropped.



13/11/2026 - 20/11/2026

    - Primero el CRUD completo (Index, Details, Create, Save, Edit, Delete)

20/11/2026 - 02/12/2026

    - personalizar controladores y vistas en base al rol del usuario autenticado.

02/12/2026 - 16/12/2026

    - Mejorar aspecto visual de las pantallas, personalizarlo para que cada proyecto se vea único
    - Opcional: quien vaya muy avanzado puede probar a agregar alguna nueva entidad
    - Ensayar presentación:
        15 min
        temática, objetivos,
        equipo
        tecnologías
        demo: mostrar rol usuario, mostrar rol admin
        aprendizajes, futuros desarrollos, conclusiones
