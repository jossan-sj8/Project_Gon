# 📌 CONTEXTO DEL PROYECTO - Project_Gon

## 🎯 Objetivo General
Sistema híbrido de manejo de stock y venta con:
- Backend: .NET 9 (ASP.NET Core)
- Frontend: Angular
- Soporte para móvil: React Native (opcional)
- Integración: Pistola de código de barras
- Integración: Caja automática de dinero

## 📊 Estado Actual
**Fecha:** 28 Enero 2026  
**Rama:** `feature/dtos-services`  
**Ambiente:** Windows, Visual Studio 2022

### ✅ COMPLETADO
- [x] Estructura modular (4 proyectos)
- [x] 21 entidades + 7 enums (DbContext)
- [x] Migration inicial (PostgreSQL)
- [x] Repository Pattern + Unit of Work
- [x] AutoMapper (solo Empresa)
- [x] EmpresasController CRUD (probado en Insomnia)
- [x] DTOs para Empresa (3: Dto, CreateDto, UpdateDto)

### 🔴 PENDIENTE - FASE 1 (CRÍTICA)
1. **Program.cs** - Configuración API
   - [ ] Agregar CORS (para Angular)
   - [ ] Agregar JWT Authentication
   - [ ] Agregar Swagger UI
   - [ ] REMOVER: WeatherForecast endpoint

2. **AuthController** - Autenticación
   - [ ] Login endpoint
   - [ ] Register endpoint

3. **FluentValidation** - Validaciones
   - [ ] Instalar paquete NuGet
   - [ ] Crear validadores

### 🟡 PENDIENTE - FASE 2
- [ ] DTOs para 20 entidades más
- [ ] AutoMapper profiles para 20 entidades
- [ ] 20 Controllers CRUD

### 🟡 PENDIENTE - FASE 3
- [ ] Servicios de negocio
- [ ] Error handling middleware
- [ ] Logging con Serilog

## 🛠️ TECNOLOGÍAS STACK
| Componente | Tecnología |
|-----------|-----------|
| Backend | .NET 9, C# 13 |
| API | ASP.NET Core |
| Database | PostgreSQL |
| ORM | Entity Framework Core 9.0.0 |
| Mapeo | AutoMapper |
| Frontend | Angular |
| Patrón | Repository + Unit of Work |

## 📂 ESTRUCTURA PROYECTOS

Project_Gon/
├── Project_Gon.Api/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Hubs/
│   └── Program.cs
├── Project_Gon.Core/
│   ├── Entities/
│   ├── DTOs/
│   ├── Enums/
│   └── Interfaces/
├── Project_Gon.Infrastructure/
│   ├── Data/
│   ├── Repositories/
│   ├── Mappings/
│   └── Services/
└── Project_Gon.Common/
    ├── Helpers/
    ├── Constants/
    └── Exceptions/