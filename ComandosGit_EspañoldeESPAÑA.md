# Git: checklist por casos

> Sustituye los textos en `<mayúsculas>` o entre `< >` por tus valores reales (sin los símbolos `< >`).

---

## 1. Subir un proyecto nuevo a GitHub

- Inicializar el repo: `git init`
- Añadir archivos: `git add .`
- Primer commit: `git commit -m "First commit"`
- Renombrar rama a main: `git branch -M main`
- Conectar con GitHub: `git remote add origin <URL>`
- Subir: `git push -u origin main`

---

## 2. Cambió mi usuario/URL del remoto

- Cambiar la URL: `git remote set-url origin <NEW_URL>`
- Verificar: `git remote -v`

---

## 3. Empezar una tarea nueva (rama nueva)

- Ir a main: `git switch main`
- Traer lo último: `git pull`
- Crear y entrar en la rama: `git switch -c <branch-name>`

> Una rama nueva por cada encargo. Nombre descriptivo: `fix-database`, `add-grades-section`...

---

## 4. Día a día en mi rama

- Ver qué ha cambiado: `git status`
- Añadir archivos concretos: `git add <file>`
- Commit: `git commit -m "#<issue-number> descripción"`
- Subir la rama (la primera vez): `git push -u origin <branch-name>`
- Subir la rama (las siguientes): `git push`

> `push` desde tu rama **no toca main**. Es copia de seguridad y visibilidad.
> Formato de commit para enlazar con un issue: `#4 mensaje`.

---

## 5. Un commit por archivo (separar cambios)

- `git status`
- `git add <file_a>`
- `git commit -m "#<issue-number> cambios en A"`
- `git add <file_b>`
- `git commit -m "#<issue-number> cambios en B"`

Extras:

- Sacar un archivo del staging sin perder cambios: `git restore --staged <file>`
- Separar cambios dentro de un mismo archivo: `git add -p <file>`
- En VS Code: pestaña Source Control, botón `+` junto a cada archivo.

---

## 6. Terminé mi tarea: integrar en main

- Commit de todo lo pendiente
- `git switch main`
- `git pull`
- `git switch <branch-name>`
- Traer main a mi rama: `git merge main`
- Resolver conflictos si los hay (ver sección 9)
- Subir mi rama: `git push -u origin <branch-name>`
- **Opción A, Pull Request:** abrir PR en GitHub, revisar, merge desde la web
- **Opción B, merge directo:**
  - `git switch main`
  - `git merge <branch-name>`
  - `git push`
- Borrar rama local: `git branch -d <branch-name>`
- Borrar rama remota (opcional): `git push origin --delete <branch-name>`

---

## 7. Urgente: subir solo algunos archivos a main (cherry-pick)

Caso: tengo cambios en A, B, C, D (sin commit) y solo C y D deben ir ya a main.

- Commit solo de C y D: `git add <file_c> <file_d>`
- `git commit -m "#<issue-number> update database"`
- Ver el hash: `git log --oneline -1` (ejemplo: `a1b2c3d`)
- Apartar A y B: `git stash`
- `git switch main`
- `git pull`
- Copiar el commit: `git cherry-pick <hash>`
- `git push`
- Volver a mi rama: `git switch <branch-name>`
- Recuperar A y B: `git stash pop`

Si hay conflicto en el cherry-pick:

- Resolver, luego `git add <file>` y `git cherry-pick --continue`
- Cancelar: `git cherry-pick --abort`

> Si en tu equipo no se puede hacer push directo a main: crea una rama desde main (`git switch -c hotfix-db`), haz el cherry-pick ahí, súbela y abre un PR.

---

## 8. Recuperar un archivo tal como está en main

- `git restore --source=main -- <path/to/file>`
- Borrar la copia que quedó en el sitio equivocado
- `git status` para comprobar
- Commit

> `merge main` **no** arregla un archivo que moviste tú: solo trae lo que cambió en main.

---

## 9. Conflictos de merge

- `git status` (lista los archivos en conflicto)
- Abrir cada archivo y buscar las marcas:

```
<<<<<<< HEAD
tu versión
=======
la versión de la otra rama
>>>>>>> otra-rama
```

- Dejar el código final y borrar las marcas
- `git add <file>`
- `git commit` (completa el merge)
- Cancelar el merge si algo va mal: `git merge --abort`

---

## 10. Corregir errores comunes

| Problema | Qué hacer |
|---|---|
| `src refspec ... does not match any` | Comprobar el nombre exacto con `git branch` y que haya al menos un commit (`git log --oneline`) |
| Añadí algo de más al staging | `git restore --staged <file>` |
| Quiero descartar cambios sin commit de un archivo | `git restore <file>` (**irreversible**) |
| No sé en qué rama estoy | `git branch` (la del `*`) |
| Cambiar de rama con cambios sin commit | Commit o `git stash` antes |

---

## 11. Stash (guardar cambios temporalmente)

- Guardar: `git stash`
- Ver la lista: `git stash list`
- Recuperar y quitar de la lista: `git stash pop`

---

## 12. Cambiar mensajes de commits ya hechos

**Solo el último commit, si aún no lo has subido:**

- `git commit --amend -m "#<issue-number> nuevo mensaje"`

**Varios commits ya subidos (reescribe historial, ¡cuidado en main compartida!):**

- Preferible: comentar los hashes en el issue (`a1b2c3d`) en vez de reescribir
- Si se acuerda con el equipo: `git rebase -i HEAD~<N>`
- Cambiar `pick` por `reword` en los commits a editar
- Editar cada mensaje
- `git push --force-with-lease`

---

## 13. Comandos de consulta útiles

| Comando | Para qué sirve |
|---|---|
| `git status` | Estado actual de archivos |
| `git branch` | Listar ramas locales |
| `git branch -a` | Listar locales y remotas |
| `git log --oneline` | Historial resumido |
| `git log --oneline --graph --all` | Historial con forma de ramas |
| `git show --stat <hash>` | Qué archivos tocó un commit |
| `git diff` | Cambios sin añadir al staging |
| `git remote -v` | URL del remoto |