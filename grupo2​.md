​
- ApplicationUser: Administrador del centro
- Departamento: Id, Nombre, Descripcion
- Profesor: Id, Nombre, Email, Telefono. Relación: Departamento 1:N
- Sede: Id, Nombre, Direccion, Ciudad
- Aula: Id, Nombre, Capacidad, Equipamento. Relación: Sede 1:N
- Curso: Id, Nombre, Nivel (emun NivelCurso: BASICO, INTERMEDIO, AVANZADO), Precio, FechaInicio, FechaFin, Estado (enum EstadoCurso: PLANIFICADO, ENCURSO, FINALIZADO). Relaciones: Departamento/Tecnología/Área N:1, Profesor N:1, Aula N:1????
- Alumno: Id, Nombre, Email, Telefono, FechaNacimiento
- Matricula: Id, Fecha, Pagada (boolean), Estado (enum EstadoMatricula: ENPROCESO, REVISANDO...). Relaciones: Alumno 1:N, Curso 1:N

Nota: Normalmente Dirección se crea como otra entidad aparte. Tipo de Via, Nombre, Codigo postal, Localidad...

ApplicationUser con Roles: ADMIN, PROFESOR, ALUMNO

