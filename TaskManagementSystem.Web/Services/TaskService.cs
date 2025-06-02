using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Web.Data;
using TaskManagementSystem.Web.Models;

namespace TaskManagementSystem.Web.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _dbContext;
        public TaskService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task CreateTaskAsync(TaskItem taskItem)
        {
            taskItem.CreatedAt = DateTime.UtcNow;
            _dbContext.Tasks.Add(taskItem);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateTaskAsync(TaskItem updatedTask)
        {
            var existingTask = await _dbContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == updatedTask.Id && t.UserId == updatedTask.UserId);
            if (existingTask != null)
            {
                existingTask.Title = updatedTask.Title;
                existingTask.Description = updatedTask.Description;
                existingTask.DueDate = updatedTask.DueDate;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteTaskAsync(int id, string userId)
        {
            var task = await _dbContext.Tasks
                 .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task != null)
            {
                _dbContext.Tasks.Remove(task);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id, string userId)
        {
            return await _dbContext.Tasks
               .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<List<TaskItem>> GetTasksForUserAsync(string userId, string filter = "all", string search = " ")
        {
            var query = _dbContext.Tasks
                .Where(t => t.UserId == userId);
            // Apply filter
            query = filter switch
            {
                "completed" => query.Where(t => t.IsCompleted),
                "incomplete" => query.Where(t => !t.IsCompleted),
                _ => query
            };
            // Apply search
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(t =>
                    t.Title.ToLower().Contains(search) ||
                    t.Description.ToLower().Contains(search));
            }
            return await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        public async Task ToggleCompleteAsync(int id, string userId)
        {
            var task = await _dbContext.Tasks
             .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;
                await _dbContext.SaveChangesAsync();
            }
        }



    }
}
