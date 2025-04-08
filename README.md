# 📦 SGE API - Sistema de Gestión Empresarial

Este proyecto corresponde a una API desarrollada en ASP.NET Core, diseñada para gestionar información de empresas, empleados, departamentos, cargos, usuarios, clientes y eventos relacionados. Forma parte del ecosistema de soluciones empresariales de **TCR Technological Consulting & Risk S.A.S.**

## 🚀 Tecnologías Utilizadas

- ASP.NET Core 7.0
- Entity Framework Core
- PostgreSQL
- JWT para autenticación
- AutoMapper
- Swagger / OpenAPI
- Docker (opcional)
- Herramientas de desarrollo: Visual Studio / Visual Studio Code

## 📁 Estructura del Proyecto

```
sge_api/
├── Controllers/        # Controladores de la API
├── Models/             # Modelos de datos (entidades)
├── Services/           # Lógica de negocio
├── Data/               # Contexto de la base de datos
├── Migrations/         # Migraciones de Entity Framework
├── Properties/
├── appsettings.json    # Configuración general del sistema
├── Program.cs          # Punto de entrada de la app
└── sge_api.sln         # Solución de Visual Studio
```

## ⚙️ Configuración Inicial

### Clonar el repositorio
```bash
git clone https://github.com/technologicalconsulting/sge_api.git
cd sge_api
```

### Aplicar las migraciones
```bash
dotnet ef database update
```

### Ejecutar la API
```bash
dotnet run
```

Accede a la documentación interactiva en Swagger:
```
http://localhost:5000/swagger
```

## 🔐 Autenticación

La API utiliza JWT para autenticación. Al iniciar sesión, se recibe un token que debe incluirse en cada petición como:

```
Authorization: Bearer {token}
```

## 📌 Funcionalidades principales

- Registro y autenticación de usuarios
- Gestión de empresas, empleados, departamentos y cargos
- Asignación de empleados a clientes
- Registro de eventos del sistema
- Validación de códigos de verificación

## 📬 Contacto

Desarrollado por [TCR Technological Consulting & Risk S.A.S.]
📧 info@technologicalconsulting.com  
📍 Ecuador

---

> Este sistema forma parte de una solución integral desarrollada por TCR S.A.S. para optimizar la gestión empresarial de nuestros clientes.
