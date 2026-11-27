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

Para los 4 grupos

    Primero haced que el flujo principal funcione de punta a punta. Las funciones nuevas, después.

    Seguridad: usad [Authorize] y comprobad en el controlador quién es el dueño del dato. Ocultar un botón no basta.

    Diseño: listados en tarjetas con imágenes, inicio distinto según el rol y todos los textos en español.

    Cada persona se encarga de una parte, sabe explicarla y sube su primer commit antes del miércoles 2.

    Subid cambios pequeños y a menudo, y fusionad pronto para evitar conflictos.

    Antes del viernes 4, ensayad la demo completa entrando con cada rol.

Grupo 2: AcademyNET
Lo que ya tenéis: el proyecto compila y arranca, tres roles, una buena portada y el recorrido matrícula, pago, examen y resultado.
Fallos que hay que corregir

    Notas: si el profesor deja la nota vacía se guarda un 0, y si escribe 7.5 se guarda 75.

    Un alumno puede crear y renombrar centros, y cambiar de curso una matrícula ya pagada.

    Un profesor puede editar o borrar cursos de otro.

    Sin iniciar sesión se ven nombres y asistencia de alumnos (/Lessons/Details/1).

    Error 500 al borrar un examen con intentos, una pregunta respondida o un usuario con matrículas.

Pendiente de terminar

    Fechas y número de intentos en el formulario de examen.

    Alta de aulas y CRUD de lecciones.

Tareas

Alfonso: dejar redondo el flujo de examen y nota (decimales, fechas, intentos y borrados) antes de añadir nada más.

Rubén: permisos: cada profesor solo sus cursos, matrículas protegidas y asistencia oculta a quien no deba verla.

Pol: proteger la gestión de centros, activar "Nueva aula" y quitar el aviso TODO. Tu rama Poulgar tiene conflictos: no la fusiones, copia a mano la vista de centro.

Juanma: CRUD de Lecciones con permisos por rol.

Pol y Rubén: tarjetas con imagen para cursos y centros, textos en español y datos de ejemplo con fechas de diciembre.
