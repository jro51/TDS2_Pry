# TDS2_Pry – Gestión de Colaboradores y Empresas

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

