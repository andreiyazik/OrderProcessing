using AutoMapper;
using OrderProcessing.Domain.DTOs.Order;
using OrderProcessing.Models.Order.Infos;

namespace OrderProcessing.Application.Mapping;

public class OrdersApplicationProfile : Profile
{
    public OrdersApplicationProfile()
    {
        CreateMap<OrderInfo, OrderCreateDto>();
        CreateMap<OrderItemInfo, OrderItemCreateDto>();
    }
}
