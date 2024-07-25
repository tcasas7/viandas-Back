using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Services.Interfaces;
using ViandasDelSur.Tools;

namespace ViandasDelSur.Services.Implementations
{
    public class MenusService : IMenusService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVerificationService _verificationService;
        private readonly ImageTool _imageTool;
        private readonly IProductRepository _productRepository;
        private readonly IImageRepository _imageRepository;
        public MenusService(
            IMenuRepository menuRepository,
            IUserRepository userRepository,
            IVerificationService verificationService,
            IProductRepository productRepository,
            IImageRepository imageRepository)
        {
            _menuRepository = menuRepository;
            _userRepository = userRepository;
            _verificationService = verificationService;
            _imageTool = new ImageTool();
            _productRepository = productRepository;
            _imageRepository = imageRepository;

        }

        public Response Get()
        {
            Response response = new Response();

            var menus = _menuRepository.GetAll();

            if (menus == null)
            {
                response.statusCode = 404;
                response.message = "Menús no encontrados";
                return response;
            }

            List<MenuDTO> result = new List<MenuDTO>();

            foreach (var menu in menus)
            {
                MenuDTO menuDTO = new MenuDTO(menu);
                result.Add(menuDTO);
            }

            response = new ResponseCollection<MenuDTO>(200, "Ok", result);

            return response;
        }

        public Response Add(string email, AddMenusDTO model)
        {
            Response response = new Response();

            var user = _userRepository.FindByEmail(email);

            if (user == null)
            {
                response.statusCode = 401;
                response.message = "Sesión invalida";
                return response;
            }

            response = _verificationService.VerifyAdmin(user);

            if (response.statusCode != 200)
                return response;

            var oldMenus = _menuRepository.GetAll();

            if (oldMenus == null)
            {
                response.statusCode = 404;
                response.message = "Elementos no encontrados";
                return response;
            }

            foreach (var menuDTO in model.Menus)
            {
                Menu menu = new Menu(menuDTO);
                menu.validDate = DatesTool.GetNextDay(DayOfWeek.Monday);
                _menuRepository.Save(menu);
            }

            foreach (var menu in oldMenus)
            {
                _menuRepository.Remove(menu);
            }

            var images =_imageRepository.GetAll();

            foreach (var image in images)
            {
                _imageTool.DeleteImage(image.route, image.name);
                _imageRepository.Remove(image);
            }

            var newMenus = _menuRepository.GetAll();

            if (newMenus == null)
            {
                response.statusCode = 404;
                response.message = "Elementos no encontrados";
                return response;
            }

            foreach (var menu in newMenus)
            {
                foreach(var menuDTO in model.Menus)
                {
                    if (menu.category == menuDTO.category)
                    {
                        foreach (var productDTO in menuDTO.products)
                        {
                            Image newImage = _imageTool.CreateForNew(productDTO.name);

                            if (newImage == null)
                            {
                                response.statusCode = 400;
                                response.message = "Error";
                                return response;
                            }

                            Product product = new Product(productDTO);
                            product.Image = newImage;
                            product.menuId = menu.Id;
                            _productRepository.Save(product);
                        }
                    }
                }
            }

            response.statusCode = 200;
            response.message = "Ok";

            return response;
        }

        public Response ChangeImage(IFormFile model, int productId)
        {
            Response response = new Response();

            // Verifica que el archivo no sea nulo
            if (model == null || model.Length == 0)
            {
                response.statusCode = 400;
                response.message = "Invalid image file.";
                return response;
            }

            var prod = _productRepository.GetById(productId);

            if (prod == null)
            {
                response.statusCode = 404;
                response.message = "Producto no encontrado";
                return response;
            }

            // Obtiene la imagen actual del usuario
            Image dbImage = _imageRepository.GetById(productId);

            try
            {
                // Crea una nueva imagen
                Image newImage = _imageTool.CreateImage(model);

                // Guarda la nueva imagen en el repositorio
                _imageRepository.Save(newImage);

                // Actualiza la referencia de la imagen en el usuario
                prod.Image = newImage;
                _productRepository.Save(prod);

                // Si hay una imagen anterior, elimínala
                if (dbImage != null)
                {
                    _imageTool.DeleteImage(dbImage.route, dbImage.name);
                    _imageRepository.Remove(dbImage);
                }

                response.statusCode = 200;
                response.message = "Profile image updated successfully.";
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = ex.Message;
            }

            return response;
        }
    }
}
