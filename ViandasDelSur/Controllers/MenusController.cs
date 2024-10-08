﻿/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Interfaces;
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
        private readonly IProductRepository _productRepository;

        public MenusController(IMenusService menusService, IProductRepository productRepository)
        {
            _menusService = menusService;
            _imageTool = new ImageTool();
            _productRepository = productRepository;
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
                var product = _productRepository.GetById(id);

                if (product == null)
                {
                    return StatusCode(404, "No encontrado");
                }

                // Asegúrate de que 'product.Image.route' contiene solo el nombre de la imagen, no "Media/nombreImagen.jpg"
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Media", product.Image.route);
                //string imagePath = "D:\\Escritorio\\viandas-Back\\ViandasDelSur\\Media\\" + product.Image.route;
                //Console.WriteLine("Ruta de la imagen: " + imagePath);



                byte[] result = _imageTool.GetImageFromPath(imagePath, "");

                if (result == null)
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
        public ActionResult<AnyType> ChangeImage(IFormFile model, int id)
        {
            Response response = new Response();
            try
            {
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                if (string.IsNullOrEmpty(email))
                {
                    response.statusCode = 400;
                    response.message = "Error";
                    return new JsonResult(response);
                }

                var prod = _productRepository.GetById(id);

                if (prod == null)
                {
                    response.statusCode = 404;
                    response.message = "Producto no encontrado";
                    return new JsonResult(response);
                }

                response = _menusService.ChangeImage(model, id);
                return new JsonResult(response);
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
            }
        }

    }
}*/



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
                // Obtener el correo electrónico del usuario
                string email = User.FindFirst("Account") != null ? User.FindFirst("Account").Value : string.Empty;

                if (string.IsNullOrEmpty(email))
                {
                    response.statusCode = 400;
                    response.message = "Error: Usuario no autenticado.";
                    return BadRequest(response);
                }

                // Llamar al servicio para cambiar la imagen
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
