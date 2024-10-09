using Microsoft.AspNetCore.Http;
using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;

namespace ViandasDelSur.Services.Interfaces
{
    public interface IMenusService
    {
        Response Get();
        Response Add(string email, AddMenusDTO model);
        Response ChangeImage(IFormFile model, int productId); // Ensure this matches the method signature in the service implementation
        Product GetProductById(int productId);
    }
}
