
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
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
                response.message = "Sesión inválida";
                return response;
            }

            response = _verificationService.VerifyAdmin(user);

            if (response.statusCode != 200)
                return response;

            // Obtenemos los menús actuales en la base de datos
            var oldMenus = _menuRepository.GetAll();

            if (oldMenus == null)
            {
                response.statusCode = 404;
                response.message = "Elementos no encontrados";
                return response;
            }

            foreach (var menuDTO in model.Menus)
            {
                // Buscar el menú actual por categoría
                var existingMenu = oldMenus.FirstOrDefault(m => m.category == menuDTO.category);

                if (existingMenu != null)
                {
                    // Actualizar los campos del menú existente
                    existingMenu.price = menuDTO.price; // Actualizar precio
                    existingMenu.precioPromo = menuDTO.precioPromo; // Actualizar precio promocional

                    // Actualizar los productos del menú existente
                    foreach (var productDTO in menuDTO.products)
                    {
                        var existingProduct = existingMenu.Products.FirstOrDefault(p => p.Id == productDTO.Id);

                        if (existingProduct != null && productDTO.Id != 0)
                        {
                            existingProduct.name = productDTO.name;
                            existingProduct.price = productDTO.price;

                            var newImage = _imageRepository.GetById(productDTO.imageId);
                            if (newImage != null)
                            {
                                existingProduct.Image = newImage;
                            }
                        }
                        else
                        {
                            var newImage = _imageRepository.GetById(productDTO.imageId);
                            var newProduct = new Product(productDTO, newImage);
                            newProduct.menuId = existingMenu.Id;
                            existingMenu.Products.Add(newProduct);
                        }
                    }

                    // Guardar los cambios en el menú existente
                    _menuRepository.Save(existingMenu);
                }
                else
                {
                    // Crear un nuevo menú si no existe
                    Menu newMenu = new Menu(menuDTO, _imageRepository)
                    {
                        validDate = DatesTool.GetNextDay(DayOfWeek.Monday),
                        precioPromo = menuDTO.precioPromo // Asignar precio promocional
                    };
                    _menuRepository.Save(newMenu);
                }
            }

            response.statusCode = 200;
            response.message = "Menús actualizados correctamente";
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

            // Obtener la imagen actual del producto
            Image dbImage = _imageRepository.GetById(productId);

            try
            {
                // Crear la nueva imagen
                Image newImage = _imageTool.CreateImage(model);

                // Guardar solo el nombre de la nueva imagen en la base de datos
                _imageRepository.Save(newImage);

                // Actualizar la referencia de la imagen en el producto
                prod.Image = newImage;
                _productRepository.Save(prod);

                // Si hay una imagen anterior, eliminarla
                if (dbImage != null)
                {
                    _imageTool.DeleteImage(dbImage.route, dbImage.name); // Eliminar la imagen física
                    _imageRepository.Remove(dbImage); // Eliminar la referencia en la base de datos
                }

                response.statusCode = 200;
                response.message = "Imagen del producto actualizada correctamente.";
            }
            catch (Exception ex)
            {
                response.statusCode = 500;
                response.message = ex.Message;
            }

            return response;
        }



        public Product GetProductById(int productId)
        {
            return _productRepository.GetById(productId);
        }
    }
}

