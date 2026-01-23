# 📋 Bitácora del Proyecto Gon

## 📅 Sesión 1 - 22 de Enero 2026

### ✅ Completado Hoy
- Creada estructura modular de 4 proyectos (.NET 8)
- Configuración de Git (ramas: main, develop, feature/core-entities)
- Instalación de paquetes NuGet (EF Core, JWT, Validation, etc.)
- Creación de 6 entidades principales:
  - Empresa
  - Sucursal
  - Usuario
  - Categoria
  - Producto
  - Stock

### 🔄 En Progreso
- Rama: eature/core-entities (ACTIVA)
- Archivos abiertos: 6 entidades
- Sin commit aún (esperando completar todas)

### ⏳ Pendiente para Completar Entidades
1. Crear 7 Enums faltantes
2. Crear 15 entidades restantes:
   - MovimientoStock
   - Cliente
   - MetodoPago
   - Venta
   - DetalleVenta
   - Pago
   - Proveedor
   - PrecioProveedor
   - CajaRegistradora
   - ArqueoCaja
   - DetalleArqueoCaja
   - Devolucion
   - DetalleDevolucion
   - AuditoriaLog
   - ModuloAcceso

### 📊 Progreso General
- Entidades: 6/21 completadas (28%)
- Enums: 0/7 completados (0%)
- Infrastructure: 0% (sin DbContext)
- API Controllers: 0% (sin controllers)

---

## 🎯 Próximos Pasos

### Sesión 2
1. Crear 7 enums en Project_Gon.Core/Enums/
2. Crear 15 entidades restantes
3. Commit: '[Características] Agregar todas las entidades y enums'
4. Merge a develop

### Sesión 3
1. Crear AppDbContext en Infrastructure
2. Configurar relaciones con Fluent API
3. Crear migraciones iniciales
4. Seed data (datos de prueba)

### Sesión 4
1. Crear interfaces IRepository<T>
2. Implementar repositories
3. Configurar DI en Program.cs

### Sesión 5
1. Crear DTOs
2. Crear controllers CRUD
3. Configurar validaciones FluentValidation

### Sesión 6
1. Implementar autenticación JWT
2. Configurar autorización
3. Error handling middleware

---

## 📝 Decisiones Tomadas

### Modelo de Datos
- ✅ Simplificado: Solo FK + relaciones necesarias
- ✅ Multi-tenant por Empresa
- ✅ Normalización 3FN
- ✅ Sin relaciones redundantes

### Enums Principales
- RolUsuario (3 roles)
- TipoMovimientoStock (4 tipos)
- TipoVenta (2 tipos)
- EstadoVenta (3 estados)
- EstadoPago (3 estados)
- EstadoArqueoCaja (2 estados)
- EstadoDevolucion (2 estados)

### Stack Tecnológico Confirmado
- .NET 8 + C# 13
- PostgreSQL (con Npgsql)
- Entity Framework Core 9.0.0
- JWT para autenticación
- FluentValidation para validaciones
- AutoMapper para DTOs
- Serilog para logging

---

## 🔗 Referencias

**Documentación de Entidades**: /docs/entities-documentation.md
**Diagrama ER**: Ver en PROGRESS.md

---

## 📌 Estado Actual por Proyecto

### Project_Gon.Api
- [ ] Program.cs configurado
- [ ] Controllers creados
- [ ] DTOs creados
- [ ] Autenticación JWT

### Project_Gon.Core
- [x] 6 Entidades creadas
- [ ] 7 Enums creados
- [ ] 15 Entidades restantes
- [ ] Interfaces de servicios

### Project_Gon.Infrastructure
- [ ] AppDbContext
- [ ] Repositories
- [ ] Migraciones
- [ ] Seed data

### Project_Gon.Common
- [ ] Helpers
- [ ] Constants
- [ ] Exceptions

---

## 🌿 Estado de Git

**Rama Actual**: feature/core-entities
**Commits Pendientes**: 1 (con todas las entidades)

\\\
main
 └── develop
      └── feature/core-entities (AQUÍ ESTAMOS)
\\\

---

## 📞 Notas Importantes

- Email en Usuario es nullable (puede no tener)
- Cliente puede ser público (EsPublico = true)
- Stock tiene StockMinimo para alertas
- Venta registra IVA separadamente
- Todos los registros tienen CreatedAt/UpdatedAt para auditoría
- Usuario puede ser AdminGlobal (sin sucursal) o asignado a sucursal

