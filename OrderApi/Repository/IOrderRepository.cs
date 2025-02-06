
using OrderApi.Model;

namespace OrderApi.Repository;

public interface IOrderRepository
{
    Task<bool> AddOrder(OrderHeader header);
    Task UpdateOrderPaymentStatus(long orderHeaderId, bool status);
}
