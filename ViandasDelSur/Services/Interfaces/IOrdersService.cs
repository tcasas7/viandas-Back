using ViandasDelSur.Models.DTOS;
using ViandasDelSur.Models.Responses;

namespace ViandasDelSur.Services.Interfaces
{
    public interface IOrdersService
    {
        Response GetDates(string adminEmail);
        Response GetAll(string adminEmail, string email);
        Response GetOwn(string email);
        public Response Place(string email, PlaceOrderDTO model);

        Response Remove(string email, int orderId);
        Response GetOrderProducts(int orderId);
        Response GetAllOrders();
        List<Product> GetProductsByOrderId(int orderId);
    }
}
