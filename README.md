## Viandas del Sur - Backend

### Proyecto

Este proyecto fue desarrollado como parte de un trabajo práctico real para un cliente del rubro alimenticio. El objetivo fue automatizar el proceso de pedidos y distribución de viandas, eliminando el manejo manual por WhatsApp, Instagram y Excel.

---

### 📊 Tecnologías utilizadas

* **.NET Core** con ASP.NET para la creación de APIs REST.
* **Entity Framework** como ORM.
* **SQL Server** como base de datos relacional.
* **Swagger** para documentación y testeo de endpoints.

---

### ✅ Funcionalidades principales

* Registro, inicio de sesión y gestión de usuarios.
* API REST para:

  * Creación y modificación de pedidos
  * Gestión de menú semanal
  * Visualización de pedidos activos
  * Asignación de roles y tareas administrativas
* Validación y prueba de endpoints con Swagger y Postman.

---

### ⚙️ Configuración y ejecución local

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/tcasas7/viandas-Back.git
   ```

2. Configurar `appsettings.json` con tu string de conexión local:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=ViandasDB;Trusted_Connection=True;"
     }
   }
   ```

3. Ejecutar desde Visual Studio o con:

   ```bash
   dotnet run
   ```

4. Ingresar a Swagger desde:

   ```
   ```

[http://localhost:5000/swagger](http://localhost:5000/swagger)

````

---

### 🌐 Seguridad y manejo de claves
> ⚠️ **Nota**: En el historial del repositorio pudo haber una Google API Key que ya fue revocada. Se mantuvo visible por motivos de simplicidad, y no representa ningún riesgo. El repositorio puede ser utilizado libremente como referencia profesional.

---

### ✨ Sobre el desarrollo
- Metodología **Scrum** con 5 sprints en total.
- Testing continuo con Postman y Swagger.
- Enfoque en automatizar una operación que antes era 100% manual.

---

