# TaskManager.API
# Sistema de Gestión de Tareas - Backend API

Esta API RESTful desarrollada con .NET 8 proporciona los servicios backend para un sistema de gestión de tareas con autenticación de usuarios y sistema de roles. La solución incluye autenticación JWT, gestión de usuarios y tareas, y un sistema de autorización basado en roles.

## Características Principales
- Autenticación segura con JWT
- Sistema de roles: Administrador, Supervisor y Empleado
- CRUD completo de usuarios y tareas
- Asignación de tareas a usuarios
- Seguimiento de estados de tareas (Pendiente, En Progreso, Completada)
- Validación de datos y manejo de errores
- Documentación API con Swagger/OpenAPI 3.0

## Tecnologías Utilizadas
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server 2022
- JWT (JSON Web Tokens)
- Swagger/OpenAPI 3.0

## Requisitos Previos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2022](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)

## Configuración Inicial

### 1. Clonar el repositorio
git clone https://github.com/achavezm94/TaskManager.API

## 2. Configurar el archivo appsettings.json
Crea o actualiza el archivo de configuración con los siguientes valores:

- "Connection": "Server=localhost\\SQLEXPRESS;Database=TaskManagerDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
- "Key": "ingresa_key_super_segura_aqui"

## 3. Crear y aplicar migraciones de base de datos
Abre la consola de Paquetes NuGet (Tools > NuGet Package Manager > Package Manager Console)
- Add-Migration InitialCreate -OutputDir "Data\Migrations" -Project "TaskManager.API"
	
## 4. Ejecutar la aplicación
dotnet run

## 5. Probar la API con Swagger UI
Accede a la interfaz de Swagger en tu navegador:
- https://localhost:5001/swagger

## 6.Documentacion de API en Swagger
- http://localhost:5045/swagger/index.html

## Estructura del proyecto

	-	TaskManager.API/
		├── Controllers/           # Controladores API
		│   ├── AuthController.cs
		│   ├── TasksController.cs
		│   └── UsersController.cs
		├── Data/                  # Configuración de base de datos
		│   │── Migrations/        # Migraciones de Entity Framework
		│   └── AppDbContext.cs
		├── DTOs/                  # Objetos de Transferencia de Datos
		│   ├── Auth/
		│   ├── Tasks/
		│   └── Users/
		├── Models/                # Entidades de dominio
		│   ├── ProjectTask.cs
		│   └── User.cs
		├── Services/              # Lógica de negocio
		│   ├── AuthService.cs
		│   ├── TaskService.cs
		│   └── UserService.cs
		├── appsettings.json       # Configuración de la aplicación
		└── Program.cs             # Punto de entrada

## Endpoints de la API
	- Autenticación
		POST /api/auth/register: Registrar nuevo usuario
		POST /api/auth/login: Iniciar sesión y obtener token JWT

	- Usuarios (Requiere rol Administrador)
		GET /api/users: Obtener todos los usuarios
		GET /api/users/{id}: Obtener usuario por ID
		POST /api/users: Crear nuevo usuario
		PUT /api/users/{id}: Actualizar usuario
		DELETE /api/users/{id}: Eliminar usuario

	- Tareas
		GET /api/tasks: Obtener tareas (según rol del usuario)
		GET /api/tasks/{id}: Obtener tarea por ID
		POST /api/tasks: Crear nueva tarea (Admin/Supervisor)
		PUT /api/tasks/{id}: Actualizar tarea (Admin/Supervisor)
		PATCH /api/tasks/{id}/status: Actualizar estado de tarea
		DELETE /api/tasks/{id}: Eliminar tarea (Admin)