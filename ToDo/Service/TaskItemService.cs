using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDo.Model;
using ToDo.Repository;

namespace ToDo.Service
{
    public class TaskItemService : ITaskItemService
    {
        private readonly ITaskItemRepository repository;
        public TaskItemService(ITaskItemRepository taskItemRepository)
        {
            repository = taskItemRepository;
        }

        public async Task AddAsync(TaskItem taskItem)
        {
            if (taskItem.SubTasks != null && taskItem.SubTasks.Count > 0)
            {
                foreach (var subTask in taskItem.SubTasks)
                {
                    if (subTask.SubTasks != null && subTask.SubTasks.Count > 0)
                    {
                        throw new InvalidOperationException("Subtasks cannot have their own subtasks.");
                    }
                    subTask.IsSubTask = true;
                }
            }
            await repository.AddAsync(taskItem);
        }

        public async Task DeleteAsync(Guid uid)
        {
            await repository.DeleteAsync(uid);
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await repository.GetAllAsync();
        }

        public async Task<TaskItem> GetByIdAsync(Guid uid)
        {
            return await repository.GetByIdAsync(uid);
        }

        public async Task UpdateAsync(TaskItem taskItem)
        {
            TaskItem existingTaskItem = await repository.GetByIdAsync(taskItem.UID);

            if (existingTaskItem.IsSubTask && taskItem.SubTasks != null && taskItem.SubTasks.Count > 0)
            {
                throw new InvalidOperationException("Subtasks cannot have their own subtasks.");
            }

            if (taskItem.SubTasks != null && taskItem.SubTasks.Count > 0)
            {
                bool allSubtasksComplete = true;
                foreach (var subTask in taskItem.SubTasks)
                {
                    if (subTask.SubTasks != null && subTask.SubTasks.Count > 0)
                    {
                        throw new InvalidOperationException("Subtasks cannot have their own subtasks.");
                    }
                    if(!subTask.Status) {
                        allSubtasksComplete = false;
                    }
                    subTask.IsSubTask = true;
                }
                
                if (allSubtasksComplete){
                    taskItem.Status = true;
                }
            }

            if (taskItem.Status && taskItem.SubTasks != null && taskItem.SubTasks.Count > 0)
            {
                foreach (var subTask in taskItem.SubTasks)
                {
                    subTask.Status = true;
                }
            }

            await repository.UpdateAsync(taskItem);
        }
    }
}