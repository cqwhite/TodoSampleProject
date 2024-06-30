using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ToDo.Model;
using ToDo.Service;

namespace ToDo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskItemService service;

        public TaskItemController(ITaskItemService taskItemService)
        {
            service = taskItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var taskItems = await service.GetAllAsync();
            return Ok(taskItems);
        }

        [HttpGet("{uid}")]
        public async Task<ActionResult<TaskItem>> GetTask(Guid uid)
        {
            var taskItem = await service.GetByIdAsync(uid);
            if (taskItem == null)
            {
                return NotFound();
            }
            return Ok(taskItem);
        }

        [HttpPost]
        public async Task<ActionResult> CreateTask(TaskItem taskItem)
        {
            await service.AddAsync(taskItem);
            return CreatedAtAction(nameof(GetTask), new { uid = taskItem.UID }, taskItem);
        }

        [HttpPut("{uid}")]
        public async Task<IActionResult> UpdateTask(Guid uid, [FromBody] TaskItem taskItem)
        {
            if (uid != taskItem.UID)
            {
                return BadRequest();
            }
            await service.UpdateAsync(taskItem);
            var updatedTask = service.GetByIdAsync(uid);
            return Ok(updatedTask);
        }

        [HttpDelete("{uid}")]
        public async Task<IActionResult> DeleteTask(Guid uid)
        {
            await service.DeleteAsync(uid);
            return NoContent();
        }

    }
}