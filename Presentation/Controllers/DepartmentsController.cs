using Application.Common.DTO;
using Application.Departments.Commands;
using Application.Employeers.Queries.DTO;
using Application.Employeers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Departments.Queries;
using Application.Departments.Queries.DTO;
using Application.Employeers.Commands;
using Presentation.DTO;

namespace Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class DepartmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [Produces(typeof(GetDepartmentDTO))]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var query = new GetDepartmentQuery(id);
        var response = await mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [Produces(typeof(PaginationDTO<GetDepartmentDTO>))]
    public async Task<IActionResult> List([FromQuery] ListDepartmentQuery query)
    {
        var response = await mediator.Send(query);

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(200)]
    [Produces(typeof(Guid))]
    public async Task<IActionResult> Create(CreateDepartmentCommand query)
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

    [HttpPut("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] string name)
    {
        var command = new UpdateDepartmentCommand
        {
            Id = id,
            Name = name
        };

        await mediator.Send(command);

        return Ok();
    }
}
