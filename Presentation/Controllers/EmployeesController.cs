using Application.Common.DTO;
using Application.Employeers.Commands;
using Application.Employeers.Queries;
using Application.Employeers.Queries.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class EmployeesController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Produces(typeof(GetEmployeeDTO))]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var query = new GetEmployeeQuery(id);
        var response = await mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [Produces(typeof(PaginationDTO<GetEmployeeDTO>))]
    public async Task<IActionResult> List([FromQuery] ListEmployeeQuery query)
    {
        var response = await mediator.Send(query);

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [Produces(typeof(Guid))]
    public async Task<IActionResult> Create([FromQuery] CreateEmployeeCommand query)
    {
        var response = await mediator.Send(query);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        var command = new DeleteEmployeeCommand(id);
        await mediator.Send(command);

        return Ok();
    }

    [HttpPut]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update([FromQuery] UpdateEmployeeCommand command)
    {
        await mediator.Send(command);

        return Ok();
    }
}
