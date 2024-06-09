using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Implementations;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Services.Implementations;
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
            Response response = new Response();
            try
            {
                var product = _productRepository.GetById(id);

                if (product == null)
                {
                    response.statusCode = 404;
                    response.message = "No encontrado";
                    return new JsonResult(response);
                }

                byte[] result = _imageTool.GetImageFromPath(product.Image.route, "Media\\Default.png");

                if (result == null)
                {
                    response.statusCode = 404;
                    response.message = "Imagen no encontrada";
                    return new JsonResult(response);
                }

                var file = File(result, "image/png");

                return file;
            }
            catch (Exception e)
            {
                response.statusCode = 500;
                response.message = e.Message;
                return new JsonResult(response);
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
