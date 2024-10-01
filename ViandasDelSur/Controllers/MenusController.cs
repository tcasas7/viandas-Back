﻿using Microsoft.AspNetCore.Authorization;
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
        public ActionResult<AnyType> ImageByProductId(int id)
        {
            try
            {
                var product = _productRepository.GetById(id);

                if (product == null)
                {
                    return StatusCode(404, "No encontrado");
                }

                // Usa 'route' para obtener la ubicación correcta de la imagen
                string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "Media", product.Image.route);
                //Console.WriteLine($"Buscando imagen en: {imagePath}");

                byte[] result = _imageTool.GetImageFromPath(imagePath, "");

                if (result == null)
                    return StatusCode(404, "Imagen no encontrada");

                var file = File(result, "image/png");

                return file;
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
}
