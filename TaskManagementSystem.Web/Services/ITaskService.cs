using TaskManagementSystem.Web.Models;

namespace TaskManagementSystem.Web.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetTasksForUserAsync(string userId, string filter,string search);
        Task<TaskItem?> GetTaskByIdAsync(int id, string userId);
        Task CreateTaskAsync(TaskItem taskItem);
        Task UpdateTaskAsync(TaskItem taskItem);
        Task DeleteTaskAsync(int id, string userId);
        Task ToggleCompleteAsync(int id, string userId);
    }
}
