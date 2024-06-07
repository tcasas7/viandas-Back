using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;

namespace ViandasDelSur.Services.Interfaces
{
    public interface IOrdersService
    {
        Response GetAll(string adminEmail, string email);
        Response GetOwn(string email);
        Response Place(string email, OrderDTO model);
        Response Remove(string email, int orderId);
    }
}
