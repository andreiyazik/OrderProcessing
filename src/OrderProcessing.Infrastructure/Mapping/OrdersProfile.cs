using AutoMapper;
using OrderProcessing.Domain.Aggregates.Order;
using OrderProcessing.Domain.DTOs.Order;
using OrderProcessing.Infrastructure.Entities;
using OrderProcessing.Models.Order.Enums;
using OrderProcessing.Models.Order.Views;

namespace OrderProcessing.Infrastructure.Mapping;

public class OrdersProfile : Profile
{
    public OrdersProfile()
    {
        CreateMap<OrderItem, OrderItemEntity>().ReverseMap();
        CreateMap<Order, OrderEntity>().ReverseMap();
        CreateMap<Order, CreatedOrderView>();
        CreateMap<OrderDto, OrderView>();
        CreateMap<OrderItemDto, OrderItemView>();
        CreateMap<Status, StatusView>()
            .ConvertUsing(src => new StatusView((int)src, src.ToString()));
    }
}
