# Oben

Aplicacion de gestion de productos para una prueba tecnica. El objetivo es demostrar una implementacion pequena pero completa con autenticacion, CRUD de productos y auditoria en SQL Server mediante triggers.

## Alcance

La solucion se limita a dos areas funcionales:

- Autenticacion de usuarios.
- Gestion de productos: listar, consultar, crear, actualizar y eliminar.

No se agregan modulos fuera del alcance de la prueba, como roles, reportes, dashboard, categorias, proveedores, carrito, inventario avanzado o gestion visual de auditoria.

## Requisitos principales

- .NET 10.
- ASP.NET Core Web API.
- .NET MAUI Blazor Hybrid.
- SQL Server.
- MediatR.
- CQRS.
- FluentValidation.
- DDD ligero.
- Acceso SQL directo.
- Mapeo manual.
- Auditoria con triggers de SQL Server.

Restricciones importantes:

- No usar Entity Framework.
- No usar AutoMapper, Mapster ni otros mappers automaticos.
- No registrar la auditoria desde la aplicacion; debe quedar a cargo de SQL Server.

## Estructura de la solucion

```text
Oben.slnx
+-- Oben.Api
+-- Oben.Application
+-- Oben.Domain
+-- Oben.Infrastructure
+-- Oben.Maui
```

Responsabilidades esperadas:

- `Oben.Domain`: entidades y reglas de dominio, especialmente `Product` y `User`.
- `Oben.Application`: commands, queries, handlers, validadores y contratos de casos de uso.
- `Oben.Infrastructure`: acceso directo a SQL Server, ejecucion de SQL y mapeo manual.
- `Oben.Api`: endpoints HTTP y composicion de dependencias del backend.
- `Oben.Maui`: cliente MAUI Blazor Hybrid con pantallas de login y productos.

## Modelo de datos

Tablas requeridas:

- `users`: usuarios usados para iniciar sesion.
- `products`: entidad principal del CRUD.
- `product_update_audit`: cambios generados por actualizaciones.
- `product_delete_audit`: registros eliminados.

Campos minimos:

```text
users
- id
- name
- email
- password

products
- id
- name
- description
- price
- stock

product_update_audit
- id
- table_name
- record_id
- field_name
- old_value
- new_value
- user_id
- created_at

product_delete_audit
- id
- table_name
- record_id
- old_value
- user_id
- created_at
```

## Auditoria

La auditoria debe implementarse con triggers de SQL Server:

- En `UPDATE products`, el trigger compara `deleted` e `inserted` y registra los cambios en `product_update_audit`.
- En `DELETE products`, el trigger usa `deleted` para registrar el estado anterior en `product_delete_audit`.

La aplicacion debe establecer el usuario autenticado en la sesion SQL antes de actualizar o eliminar:

```sql
EXEC sp_set_session_context @key = N'UserId', @value = @UserId;
```

Los triggers deben leer ese valor con:

```sql
SESSION_CONTEXT(N'UserId')
```

## CQRS y casos de uso

Commands esperados:

- `LoginCommand`
- `CreateProductCommand`
- `UpdateProductCommand`
- `DeleteProductCommand`

Queries esperadas:

- `GetProductsQuery`
- `GetProductByIdQuery`

Las entradas de commands se validan con FluentValidation. Las reglas deben permanecer simples y alineadas con el dominio:

- Nombre de producto requerido.
- Precio mayor que cero.
- Stock mayor o igual que cero.
- Email y password requeridos para login.

## Interfaz

La interfaz se construye en `Oben.Maui` con .NET MAUI Blazor Hybrid. Las pantallas esperadas son:

- Login.
- Listado de productos.
- Formulario de creacion y edicion.
- Confirmacion o accion de eliminacion.

Radzen Blazor puede evaluarse como libreria de componentes UI equivalente a PrimeNG para Blazor Hybrid.

## Comandos utiles

Restaurar paquetes:

```powershell
dotnet restore .\Oben.slnx
```

Compilar solucion:

```powershell
dotnet build .\Oben.slnx
```

Ejecutar API:

```powershell
dotnet run --project .\Oben.Api\Oben.Api.csproj
```

Swagger queda disponible en:

```text
https://localhost:7212/swagger
http://localhost:5277/swagger
```

Configurar SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=oben;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Crear tablas y triggers:

```powershell
sqlcmd -S localhost -d Oben -i .\Oben.Infrastructure\Sql\Database.sql
```

Insertar datos demo opcionales:

```powershell
sqlcmd -S localhost -d Oben -i .\Oben.Infrastructure\Sql\Database.Seed.sql
```

Ejecutar MAUI en Windows:

```powershell
dotnet build .\Oben.Maui\Oben.Maui.csproj -f net10.0-windows10.0.19041.0
```

## Criterios de aceptacion

Al finalizar, el evaluador debe poder:

- Iniciar la API y la aplicacion MAUI.
- Iniciar sesion con un usuario existente.
- Listar productos.
- Crear un producto.
- Editar un producto.
- Eliminar un producto.
- Verificar en SQL Server que las actualizaciones generan registros con `old_value`, `new_value` y `user_id`.
- Verificar en SQL Server que las eliminaciones generan registros con `old_value` y `user_id`.
- Confirmar en el codigo el uso de CQRS, MediatR, FluentValidation, DDD ligero, SQL directo y mapeo manual.
