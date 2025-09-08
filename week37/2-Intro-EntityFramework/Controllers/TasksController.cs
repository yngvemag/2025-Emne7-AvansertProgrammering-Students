using Intro_EntityFramework.Data;
using Intro_EntityFramework.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Intro_EntityFramework.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly TaskDbContext _taskDbContext;
    public TasksController(TaskDbContext taskDbContext)
    {
        _taskDbContext = taskDbContext;
    }
    
    // GET: api/<TasksController>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasksAsync()
    {
        return await _taskDbContext.Tasks.ToListAsync();
    }

    // GET api/<TasksController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<TasksController>
    [HttpPost]
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<TasksController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<TasksController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}

