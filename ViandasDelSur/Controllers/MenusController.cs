
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using System.IO;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Services.Interfaces;
using ViandasDelSur.Tools;

namespace ViandasDelSur.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenusController : ControllerBase
    {
        private readonly IMenusService _menusService;
        private readonly ImageTool _imageTool;

        public MenusController(IMenusService menusService)
        {
            _menusService = menusService;
            _imageTool = new ImageTool();
        }

        [HttpGet]
        public ActionResult<AnyType> Get()
        {
            Response response = new Response();

            try
            {
                response = _menusService.Get();
                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
            }
        }

        [Authorize]
        [HttpPost("add")]
        public ActionResult<AnyType> Add([FromBody] AddMenusDTO model)
        {
            Response response = new Response();

            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                response = _menusService.Add(email, model);
                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
            }
        }

        [HttpGet("image/{id}")]
        public ActionResult ImageByProductId(int id)
        {
            try
            {
                var product = _menusService.GetProductById(id);
                if (product == null)
                {
                    return StatusCode(404, "Producto no encontrado");
                }
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Media", product.Image.name);
                //string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Media", product.Image.route);
                byte[] result = _imageTool.GetImageFromPath(imagePath, "");

                if (result == null || result.Length == 0)
                {
                    return StatusCode(404, "Imagen no encontrada");
                }

                return File(result, "image/png");
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [Authorize]
        [HttpPost("changeImage/{id}")]
        public IActionResult ChangeImage(int id, IFormFile file)
        {
            Response response = new Response();

            try
            {
                // 🔍 Verificar si el usuario está autenticado
                string email = User.FindFirst("Account")?.Value ?? string.Empty;

                if (string.IsNullOrEmpty(email))
                {
                    response.statusCode = 400;
                    response.message = "Error: Usuario no autenticado.";
                    return BadRequest(response);
                }

                // 🔍 Verificar si se subió un archivo
                if (file == null || file.Length == 0)
                {
                    response.statusCode = 400;
                    response.message = "Error: No se ha subido ningún archivo.";
                    return BadRequest(response);
                }

                // 🔍 Validar que el archivo sea JPG
                var allowedExtensions = new List<string> { ".jpg", ".jpeg" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    response.statusCode = 400;
                    response.message = "Error: Solo se permiten archivos en formato JPG.";
                    return BadRequest(response);
                }

                // 🚀 Llamar al servicio para cambiar la imagen
                response = _menusService.ChangeImage(file, id);
                return StatusCode(response.statusCode, response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return StatusCode(500, response);
            }
        }


    }
}
