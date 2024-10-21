using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;

namespace ViandasDelSur.Services.Interfaces
{
    public interface IOrdersService
    {
        Response GetDates(string adminEmail);
        Response GetAll(string adminEmail, string email);
        Response GetOwn(string email);
        Response Place(string email, ICollection<OrderDTO> model);
        Response Remove(string email, int orderId);
        Response GetOrderProducts(int orderId);
        List<Product> GetProductsByOrderId(int orderId);
    }
}
