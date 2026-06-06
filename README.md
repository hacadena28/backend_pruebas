# Vehicle Management API - Backend

Este es el backend del sistema de Gestión de Vehículos, desarrollado en **.NET 8** siguiendo los principios de Clean Architecture. Utiliza **MongoDB** como base de datos y **JWT** para la autenticación.

## 🛠️ Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Necesario para las pruebas de rendimiento y SonarQube)
*   [MongoDB](https://www.mongodb.com/try/download/community) (Puede ser local o via Docker)

## 🚀 Configuración y Arranque

### 1. Variables de Entorno
Crea un archivo `.env` en la carpeta `VehicleManagement.Api/` con el siguiente contenido:

```env
PORT=5014
ASPNETCORE_ENVIRONMENT=Development

# MongoDB
MONGODB_CONNECTION_STRING=mongodb://localhost:27017
MONGODB_DATABASE_NAME=VehicleManagement

# JWT Security
JWT_ISSUER=VehicleManagementAPI
JWT_AUDIENCE=VehicleManagementFrontend
JWT_SIGNING_KEY=EstaEsUnaClaveSuperSecretaYMuyLargaDeAlMenos32Caracteres
JWT_EXPIRE_MINUTES=60
```

### 2. Ejecutar la API
Desde la raíz del proyecto, ejecuta:

```powershell
dotnet restore
dotnet run --project VehicleManagement.Api
```
La API estará disponible en `http://localhost:5014`. Puedes acceder a Swagger en `http://localhost:5014/swagger` (si está habilitado).

---

## 🧪 Pruebas Automatizadas

### 1. Pruebas Unitarias e Integración (xUnit)
El proyecto incluye un conjunto de pruebas automáticas para validar la lógica de negocio. Para ejecutarlas:

```powershell
dotnet test
```

### 2. Pruebas de Rendimiento (JMeter)
Se han configurado pruebas de carga para validar la resistencia del backend bajo estrés. Estas pruebas simulan el flujo de Login y operaciones CRUD de vehículos.

**Requisito:** El proyecto debe estar en una ruta fuera de OneDrive (ej: `C:\Repos\backend_pruebas`) para evitar conflictos de escritura con Docker.

**Cómo ejecutar:**
1. Asegúrate de que la API esté corriendo en el puerto `5014`.
2. En una terminal de PowerShell, ejecuta:
   ```powershell
   .\run-performance-tests.ps1
   ```

**¿Qué hace este script?**
*   Limpia resultados anteriores.
*   Inicia un contenedor Docker con **Apache JMeter**.
*   Simula 10 usuarios concurrentes durante 60 segundos.
*   Genera un reporte visual en `jmeter/report/`.
*   Abre automáticamente el reporte en tu navegador al finalizar.

---

## 🔍 Análisis de Código (SonarQube)

Para realizar un análisis de calidad de código:
1. Inicia SonarQube usando Docker Compose (proporcionado en el repositorio de frontend).
2. Ejecuta el escaneo de Sonar:
   ```powershell
   dotnet sonarscanner begin /k:"VehicleManagement.Backend" /d:sonar.host.url="http://localhost:9000" /d:sonar.token="TU_TOKEN"
   dotnet build
   dotnet sonarscanner end /d:sonar.token="TU_TOKEN"
   ```

---

## 📂 Estructura del Proyecto

*   **VehicleManagement.Api**: Capa de entrada (Controllers, Configuración).
*   **VehicleManagement.Application**: Lógica de negocio y Casos de Uso.
*   **VehicleManagement.Domain**: Entidades, Interfaces y Lógica central.
*   **VehicleManagement.Infrastructure**: Implementación de base de datos, JWT y servicios externos.
*   **VehicleManagement.Test**: Pruebas automatizadas.
*   **jmeter**: Plan de pruebas de rendimiento y reportes.
