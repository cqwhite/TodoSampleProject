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
                    //check if a subTask has any subTasks
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

            //checking if the updated subTask has any subtasks
            if (existingTaskItem.IsSubTask && taskItem.SubTasks != null && taskItem.SubTasks.Count > 0)
            {
                throw new InvalidOperationException("Subtasks cannot have their own subtasks.");
            }

            if (taskItem.SubTasks != null && taskItem.SubTasks.Count > 0)
            {
                //if the pre updated task is already complete and the updated task is now not complete, make subtasks incomplete as well
                if (existingTaskItem.isComplete && !taskItem.isComplete)
                {
                    foreach (var subTask in taskItem.SubTasks)
                    {
                        subTask.isComplete = false;
                    }

                }

                //if all subTasks are now complete make the parent task complete
                bool allSubtasksComplete = true;
                foreach (var subTask in taskItem.SubTasks)
                {
                    if (subTask.SubTasks != null && subTask.SubTasks.Count > 0)
                    {
                        throw new InvalidOperationException("Subtasks cannot have their own subtasks.");
                    }
                    if (!subTask.isComplete)
                    {
                        allSubtasksComplete = false;

                    }
                    subTask.IsSubTask = true;
                }

                if (allSubtasksComplete)
                {
                    taskItem.isComplete = true;
                }
            }

            //if the parent task is now complete make all subTasks complete
            if (taskItem.isComplete && taskItem.SubTasks != null && taskItem.SubTasks.Count > 0)
            {
                foreach (var subTask in taskItem.SubTasks)
                {
                    subTask.isComplete = true;
                }
            }

            //if the parent task was complete and a subTask is now not, make the parent task incomplete
            bool subTasksBecameIncomplete = true;
            if (existingTaskItem.isComplete && taskItem.SubTasks != null && taskItem.SubTasks.Count > 0)
            {
                foreach (var subTask in taskItem.SubTasks)
                {
                    if (!subTask.isComplete)
                    {
                        subTasksBecameIncomplete = false;

                    }
                }
                if (subTasksBecameIncomplete)
                {
                    taskItem.isComplete = false;
                }

            }

            await repository.UpdateAsync(taskItem);
        }
    }
}