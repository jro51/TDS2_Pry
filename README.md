# TDS2_Pry – Gestión de Colaboradores y Empresas

[![CI](https://github.com/jro51/TDS2_Pry/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/jro51/TDS2_Pry/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)
![Tests](https://img.shields.io/badge/tests-xUnit-green)

Aplicación web desarrollada en **ASP.NET Core MVC (.NET 10)** para administrar los **colaboradores** de una empresa, con autenticación de usuarios, una **API REST** documentada con Swagger y un **reporte RDLC/SSRS**. Proyecto del curso **TDS2** (Taller de Desarrollo de Software 2), ciclo 2026-I.

---

## Finalidad del proyecto

El proyecto tiene un fin **académico y formativo**: aplicar de forma integrada los conceptos del curso construyendo una aplicación completa, desde la base de datos hasta la interfaz y los servicios para terceros. Simula un sistema interno de una empresa (la base de datos se llama `DBIndustriasJFTDS_II_26_I`) donde se registra y consulta al personal, y se asocia cada colaborador a la empresa a la que pertenece.

Con él se busca practicar:

- El patrón **MVC** (Modelo–Vista–Controlador).
- El acceso a datos con **Entity Framework Core** (Code First y migraciones) sobre **SQL Server**.
- La separación en capas mediante **interfaces** e **inyección de dependencias**.
- La **seguridad** con ASP.NET Core Identity y roles.
- La exposición de **servicios REST** consumibles por un cliente externo (Angular).
- La generación de **reportes** con Reporting Services.

---

## Lo que se logró

### Aplicación web MVC
- **CRUD completo de Colaboradores**: listar, registrar, ver detalle, editar y eliminar.
- **Listado paginado** (8 registros por página, ordenado del más reciente al más antiguo) usando `X.PagedList`.
- Una segunda vista de listado (`ListadoColaboradorVB`) que pasa los datos mediante `ViewBag`, para comparar ambas técnicas.
- **Validaciones** con Data Annotations y mensajes en español (campos obligatorios y longitudes máximas).
- Registro automático de **fecha de creación** y **fecha de última modificación**.

### Modelo de datos
- Dos entidades relacionadas **1 a N**: `Empresa` (razón social, RUC, dirección) → `Colaborador` (nombres, apellidos, DNI, sexo, dirección).
- Base de datos creada con **migraciones de EF Core** (`ScriptManagerV1`, `ScriptManagerV2`) junto al esquema de Identity.

### Seguridad
- Registro e inicio de sesión con **ASP.NET Core Identity** (con confirmación de cuenta).
- Módulo de colaboradores protegido con `[Authorize]`.
- Definidas las políticas por rol **Jefe**, **Asistente** y **Jefe/Asistente**.

### API REST
- Dos controladores de API sobre `api/...` con operaciones de listar, consultar por id, registrar, editar y eliminar colaboradores, además del listado de empresas.
- **Swagger / OpenAPI** disponible en entorno de desarrollo.
- **CORS** configurado para un frontend **Angular** en `http://localhost:4200`.

### Reportes
- Proyecto `PIReportes` (Report Server) con el reporte **`RptColaborador.rdl`**, que lista colaboradores junto con la razón social de su empresa.

---

## Cómo ejecutarlo

**Requisitos:** [.NET 10 SDK](https://dotnet.microsoft.com/download) y SQL Server (LocalDB o Express).

```bash
git clone https://github.com/jro51/TDS2_Pry.git
cd TDS2_Pry/TDS2_Clase01

# 1) Configurar la cadena de conexión (no se guarda en el repositorio)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\\SQLEXPRESS;Database=DBIndustriasJFTDS_II_26_I;User Id=TU_USUARIO;Password=TU_CLAVE;TrustServerCertificate=True"

# 2) Crear la base de datos con las migraciones
dotnet ef database update

# 3) Ejecutar
dotnet run
```

Si usas LocalDB no necesitas el paso 1: `appsettings.json` ya trae una cadena de conexión de ejemplo para `(localdb)\mssqllocaldb`.
También puedes usar la variable de entorno `ConnectionStrings__DefaultConnection`.

## API REST y Swagger

Con la app en modo *Development*, la documentación interactiva está en **`/swagger`**.

| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/ColaboradorAPI/listar` | Lista los colaboradores |
| GET | `/api/ColaboradorAPI/listarEmpresa` | Lista las empresas |
| GET | `/api/ColaboradorAPI/detalle/{id}` | Detalle de un colaborador |
| POST | `/api/ColaboradorAPI` | Registra un colaborador |
| PUT | `/api/ColaboradorAPI/editar/{id}` | Edita un colaborador |
| DELETE | `/api/ColaboradorAPI/eliminar/{id}` | Elimina un colaborador |

Cada endpoint documenta sus códigos de respuesta (200, 400, 404, 500) y los modelos incluyen ejemplos. Los errores devuelven `{ "mensaje": "..." }`.

## Pruebas

```bash
dotnet test TDS2_Clase01.Tests/TDS2_Clase01.Tests.csproj
```

El proyecto `TDS2_Clase01.Tests` usa **xUnit**, **NSubstitute** y **EF Core InMemory**, y cubre tres niveles:

- **Acceso a datos** (`Data/`): `DAColaborador` y `DAEmpresa` contra una base en memoria.
- **Controladores** (`Controllers/`): pruebas unitarias del controlador MVC y de la API con dependencias simuladas.
- **Integración** (`Integration/`): la aplicación completa en memoria con `WebApplicationFactory` (endpoints HTTP, documento Swagger y redirección al login).

## Integración continua

En cada *push* a `master` y en cada *pull request*, **GitHub Actions** (`.github/workflows/ci.yml`) restaura, compila, ejecuta los tests y publica un resumen de cobertura. **Dependabot** propone las actualizaciones de paquetes NuGet y de las acciones.

## Arquitectura

```
Controllers (MVC + API)  ──►  Interfaces (IDAColaborador, IDAEmpresa)
                                      │
                              Acceso a datos (DAColaborador, DAEmpresa)
                                      │
                          ApplicationDbContext (EF Core + Identity)  ──►  SQL Server
```

Los controladores dependen de las **interfaces**, que se resuelven por **inyección de dependencias** (servicios *scoped* que comparten el `DbContext` de cada petición). Eso permite sustituirlas por dobles de prueba en los tests.
