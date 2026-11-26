​
# GRUPO 2 - ACADEMIAS
## 1. Resumen

- Es una aplicación para la gestión de academias, con los diferentes centros, los cursos de cada centro, las lecciones de cada curso, matriculaciones de alumnos, asistencias, exámenes, notas y, por supuesto, administración de usuarios.
- Roles de usuario y qué puede hacer cada uno:
  - Administrador: Tiene acceso a todo el sistema y todas las acciones.
  - Gestor: Tiene los mismos accesos que el administrador, excepto la gestión de usuarios.
  - Profesor: Tiene los mismos accesos que el gestor, excepto la gestión de centros y cursos.
  - Alumno: Tiene acceso a ver los centros, cursos, lecciones, matricularse, pagar, realizar sus exámenes, ver sus notas y abandonar el curso.
  - Público: Tiene acceso a ver los centros, cursos, lecciones y registrarse.
  -El registro por defecto es como alumno, el administrador, cuando sea necesario, podrá cambiar el rol al que se requiera.
- Integrantes del grupo:
    - Rubén Bitrián.
    - Alfonso Vázquez.
    - Pol García.
    - Juanma Gonzalez.


## 2. Qué tenemos hecho

Lo que funciona ahora mismo.

- CRUD básico de `User`
- CRUD básico de `Academy`
- CRUD básico de `Attendance`
- CRUD básico de `Classroom`
- CRUD básico de `Courses`
- CRUD básico de `Enrollments`
- CRUD básico de `Grades`
- CRUD básico de `Lesson`


## 3. Errores y cosas a medias

### Errores

- Nada

### Cosas a medias
- Estilo y diseño


## 4. Qué falta de lógica de negocio para MVP

### Funcionalidad Academias

- Qué hace: gestiona las academias que es la parte mas alta de gestion
- Reglas: Cada academia esta gestionada por un gestor asociado a esa academia. Cada academia tiene varios cursos.
- Modelo: 
- Controlador y acciones: 
- Vistas: 
- Pol

### Funcionalidad Cursos

- Qué hace: gestiona los cursos que cuelgan de las academias
- Reglas: Cada curso tiene varias lecciones, un profesor (pendiente de ver si más de uno), tiene varias clases y notas asociadas. Los alumnos se matriculan mediante la funcionalidad "Matriculacion"
- Modelo: 
- Controlador y acciones: 
- Vistas: 
- Ruben

### Funcionalidad Matriculacion

- Qué hace: gestiona las matriculaciones de los alumnos (pendiente de ver si apartado de profesores)
- Reglas: Cada matricula asocia la posibilidad de que un alumno pueda entrar y usar las funcionalidades de un curso y lo que cuelga de él
- Modelo: 
- Controlador y acciones: 
- Vistas: 
- Alfonso

Me cansé, que siga otro


## 5. Qué más nos falta

- Validaciones, permisos, diseño, datos de prueba, README...


## 6. Reparto por persona

### Alfonso
- [ ] Ocultar botones de Matricula, Examen, Notas para no logeados
- [ ] La ocultacion de botones esta reflejada en Controllers? Debe estar en Controllers?

### Ruben
- [ ] Detalles de curso abierto a todos. Ocultar cierta informacion a discrecion de Ruben.
- [ ] Vista para gestor donde para añadir profesor a curso 
- [ ] La ocultacion de botones esta reflejada en Controllers? Debe estar en Controllers?

### Pol
- [ ] Ocultar Boton Aulas para no logeados
- [ ] Academias abierto a todos
- [ ] Detalles Academia abierto a todos, pero aulas oculto y solo visible para usuarios con dichas aulas.
- [ ] La ocultacion de botones esta reflejada en Controllers? Debe estar en Controllers?

### Juanma
- [ ] Ocultar Boton lecciones para no logeados.
- [ ] Lecciones visibles segun logging. (Solo te aparecen las lecciones de los cursos donde estas matriculado.)
- [ ] La ocultacion de botones esta reflejada en Controllers? Debe estar en Controllers?
- [ ] Crear Rol gestor.
    - [ ] Modificar la autorizacion por roles o avisar de hacerlo para toda entidad



### Pendiente
- El gestor va asociado a academia?
- Hay cursos sin academia?
- Hay cursos que pueden estar en varias academias al mismo tiempo?
- Hay varios profesores por curso?
- Subscripciones y planes de pago?


### Propuestas
- Alfonso:
    - 

- Ruben:
    - 

- Juanma:
    - 

- Pol:
    - 



## 7. Si sobra tiempo

- Ideas tras terminar el MVP


## 8. Dudas, ruegos y preguntas

- 

