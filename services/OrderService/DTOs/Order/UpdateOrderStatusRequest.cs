
using System.ComponentModel.DataAnnotations;
using OrderService.Models;

namespace OrderService.DTOs.Order;

public class UpdateOrderStatusRequest
{
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus Status { get; set; }
}
