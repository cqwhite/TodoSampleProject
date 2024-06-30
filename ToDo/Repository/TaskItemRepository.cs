using System.Text.Json;
using ToDo.Model;

namespace ToDo.Repository
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly string filePath = "./local_data/taskItem.json";
        private List<TaskItem> taskItems;
        public TaskItemRepository()
        {
            var jsonData = File.ReadAllText(filePath);
            taskItems = JsonSerializer.Deserialize<List<TaskItem>>(jsonData) ?? new List<TaskItem>();
        }

        private async Task SaveAsync()
        {
            var jsonData = JsonSerializer.Serialize(taskItems, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(filePath, jsonData);
        }

        public async Task AddAsync(TaskItem taskItem)
        {
            taskItem.UID = Guid.NewGuid();
            taskItems.Add(taskItem);
            await SaveAsync();
        }

        public async Task DeleteAsync(Guid uid)
        {
            var taskItem = taskItems.FirstOrDefault(x => x.UID == uid);
            if (taskItem != null)
            {
                taskItems.Remove(taskItem);
                await SaveAsync();
            }
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await Task.FromResult(taskItems);
        }

        public async Task<TaskItem> GetByIdAsync(Guid uid)
        {
            var taskItem = taskItems.FirstOrDefault(t => t.UID == uid);
            return await Task.FromResult(taskItem);
        }

        public async Task UpdateAsync(TaskItem taskItem)
        {
            Console.WriteLine(taskItem.UID);
            var existingTaskItem = taskItems.FirstOrDefault(x => x.UID == taskItem.UID);
            Console.WriteLine(existingTaskItem.UID);

            if (existingTaskItem != null)
            {
                existingTaskItem.Description = taskItem.Description;
                existingTaskItem.Status = taskItem.Status;
                existingTaskItem.DeadlineDate = taskItem.DeadlineDate;
                existingTaskItem.MoreDetails = taskItem.MoreDetails;
                existingTaskItem.SubTasks = taskItem.SubTasks;
                await SaveAsync();
            }
        }
    }
}