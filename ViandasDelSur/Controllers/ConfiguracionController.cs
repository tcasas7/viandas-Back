using Microsoft.AspNetCore.Mvc;
using ViandasDelSur.Repositories.Interfaces;

namespace ViandasDelSur.Controllers
{
    [ApiController]
    [Route("api/configuracion")]
    public class ConfiguracionController : ControllerBase
    {
        private readonly IConfiguracionRepository _configRepo;

        public ConfiguracionController(IConfiguracionRepository configRepo)
        {
            _configRepo = configRepo;
        }

        [HttpGet("minimo-platos-descuento")]
        public IActionResult GetMinimoPlatosDescuento()
        {
            var config = _configRepo.GetConfiguracion();
            return Ok(new { MinimoPlatosDescuento = config.MinimoPlatosDescuento });
        }

        [HttpPost("minimo-platos-descuento")]
        public IActionResult UpdateMinimoPlatosDescuento([FromBody] int nuevoValor)
        {
            if (nuevoValor < 1) return BadRequest("El valor debe ser mayor a 0.");

            _configRepo.UpdateMinimoPlatosDescuento(nuevoValor);
            return Ok(new { message = "Configuración actualizada correctamente." });
        }
    }

}
