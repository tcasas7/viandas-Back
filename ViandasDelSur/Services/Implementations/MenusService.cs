using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ViandasDelSur.Models;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;
using ViandasDelSur.Repositories.Interfaces;
using ViandasDelSur.Services.Interfaces;

namespace ViandasDelSur.Services.Implementations
{
    public class MenusService : IMenusService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVerificationService _verificationService;

        public MenusService(
            IMenuRepository menuRepository,
            IUserRepository userRepository,
            IVerificationService verificationService)
        {
            _menuRepository = menuRepository;
            _userRepository = userRepository;
            _verificationService = verificationService;
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
                _menuRepository.Save(menu);
            }

            foreach (var menu in oldMenus)
            {
                _menuRepository.Remove(menu);
            }

            response.statusCode = 200;
            response.message = "Ok";

            return response;
        }
    }
}
