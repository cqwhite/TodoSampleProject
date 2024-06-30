using System.ComponentModel.DataAnnotations;

namespace ToDo.Model
{
    public class TaskItem
    {
        public Guid UID { get; set; }
        [Required]
        public String Description { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public DateTime? DeadlineDate { get; set; }
        public String? MoreDetails { get; set; }
        public List<TaskItem>? SubTasks { get; set; }
        public bool IsSubTask { get; set; }

    }
}