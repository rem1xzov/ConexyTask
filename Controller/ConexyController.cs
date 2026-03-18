using ConexyTask.Contract;
using ConexyTask.Model;
using ConexyTask.Service;
using Microsoft.AspNetCore.Mvc;

namespace ConexyTask.Controller;

[ApiController]
[Route("api/tasks")]
public class ConexyController : ControllerBase
{
    private readonly IConexyService _conexyService;

    public ConexyController(IConexyService conexyService)
    {
        _conexyService = conexyService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ConexyResponse>>> Get()
    {
        var to_Do_Flip = await _conexyService.GetAllTask();

        var response = to_Do_Flip.Select(t => new ConexyResponse(t.Id, t.Name, t.Description));

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ConexyResponse>> CreateTask([FromBody] ConexyRequest request)
    {
        var (to_Do, error) = Conexy.Create(
            Guid.NewGuid(),
            request.Name,
            request.Description);

        if (!string.IsNullOrEmpty(error))
        {
            return BadRequest(error);
        }

        var taskId = await _conexyService.CreateTask(to_Do);

        var response = new ConexyResponse(to_Do.Id, to_Do.Name, to_Do.Description);

        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Guid>> UpdateTask(Guid id, [FromBody] ConexyRequest request)
    {
        var to_Do_Flip = await _conexyService.UpdateTask(id, request.Name, request.Description);

        return Ok(to_Do_Flip);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<Guid>> DeleteTask(Guid id)
    {
        var to_Do_Flip = await _conexyService.DeleteTask(id);

        return Ok(to_Do_Flip);
    }
}    