using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Application.Commands;
using OrderProcessing.Application.Queries;
using OrderProcessing.Models.Order.Infos;
using OrderProcessing.Models.Order.Views;

namespace OrderProcessing.API.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public OrdersController(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<OrderView>> CreateOrder([FromBody] OrderInfo info)
    {
        var result = await _mediator.Send(new CreateOrderCommand(info));
        var createdOrderView = _mapper.Map<CreatedOrderView>(result);

        return CreatedAtAction(nameof(CreateOrder), new { id = result.Id }, createdOrderView);
    }

    [HttpGet("{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderView>> GetOrderById(Guid orderId)
    {
        var order = await _mediator.Send(new GetOrderQuery(orderId));
        var result = _mapper.Map<OrderView>(order);

        return Ok(result);
    }

    [HttpGet("{orderId}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StatusView>> GetOrderStatus(Guid orderId)
    {
        var status = await _mediator.Send(new GetOrderStatusQuery(orderId));
        var result = _mapper.Map<StatusView>(status);

        return Ok(result);
    }

    [HttpPost("{orderId}/confirm")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConfirmOrder(Guid orderId)
    {
        await _mediator.Send(new ConfirmOrderCommand(orderId));

        return NoContent();
    }
}
