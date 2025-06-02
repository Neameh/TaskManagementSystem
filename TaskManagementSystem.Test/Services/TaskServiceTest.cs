using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Web.Data;
using TaskManagementSystem.Web.Models;
using TaskManagementSystem.Web.Services;
using Xunit;

namespace TaskManagementSystem.Test.Services
{
    public class TaskServiceTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TaskService _taskService;

        public TaskServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _taskService = new TaskService(_dbContext);
        }

        [Fact]
        public async Task GetTasksForUserAsync_ReturnsTasksForUser()
        {
            // Arrange
            string userId = "test-user";
            _dbContext.Tasks.Add(new TaskItem { Title = "Test Task", UserId = userId });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskService.GetTasksForUserAsync(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Task", result[0].Title);
        }

        [Fact]
        public async Task CreateTaskAsync_AddsNewTask()
        {
            // Arrange
            var task = new TaskItem { Title = "New Task", UserId = "user1" };

            // Act
            await _taskService.CreateTaskAsync(task);

            // Assert
            var result = await _dbContext.Tasks.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal("New Task", result.Title);
        }

        [Fact]
        public async Task GetTaskByIdAsync_ReturnsCorrectTask()
        {
            // Arrange
            var task = new TaskItem { Title = "Specific Task", UserId = "user1" };
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskService.GetTaskByIdAsync(task.Id, "user1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Specific Task", result.Title);
        }

        [Fact]
        public async Task UpdateTaskAsync_UpdatesExistingTask()
        {
            // Arrange
            var task = new TaskItem { Title = "Old Title", UserId = "user1" };
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            // Modify
            task.Title = "Updated Title";

            // Act
            await _taskService.UpdateTaskAsync(task);

            // Assert
            var result = await _dbContext.Tasks.FirstOrDefaultAsync();
            Assert.Equal("Updated Title", result.Title);
        }

        [Fact]
        public async Task DeleteTaskAsync_RemovesTask()
        {
            // Arrange
            var task = new TaskItem { Title = "To Delete", UserId = "user1" };
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            // Act
            await _taskService.DeleteTaskAsync(task.Id, "user1");

            // Assert
            var result = await _dbContext.Tasks.FindAsync(task.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task ToggleCompleteAsync_ChangesStatus()
        {
            // Arrange
            var task = new TaskItem { Title = "Toggle", UserId = "user1", IsCompleted = false };
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            // Act
            await _taskService.ToggleCompleteAsync(task.Id, "user1");

            // Assert
            var result = await _dbContext.Tasks.FindAsync(task.Id);
            Assert.True(result.IsCompleted);
        }
        [Fact]
        public async Task GetTasksForUserAsync_FilterCompleted_ReturnsOnlyCompleted()
        {
            // Arrange
            string userId = "filter-user";
            _dbContext.Tasks.AddRange(
                new TaskItem { Title = "Task 1", IsCompleted = true, UserId = userId },
                new TaskItem { Title = "Task 2", IsCompleted = false, UserId = userId }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskService.GetTasksForUserAsync(userId, "completed", null);

            // Assert
            Assert.Single(result);
            Assert.True(result.All(t => t.IsCompleted));
        }

        [Fact]
        public async Task GetTasksForUserAsync_FilterIncomplete_ReturnsOnlyIncomplete()
        {
            // Arrange
            string userId = "filter-user";
            _dbContext.Tasks.AddRange(
                new TaskItem { Title = "Task A", IsCompleted = true, UserId = userId },
                new TaskItem { Title = "Task B", IsCompleted = false, UserId = userId }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskService.GetTasksForUserAsync(userId, "incomplete", null);

            // Assert
            Assert.Single(result);
            Assert.True(result.All(t => !t.IsCompleted));
        }

        [Fact]
        public async Task GetTasksForUserAsync_SearchTitle_ReturnsMatchingTasks()
        {
            // Arrange
            string userId = "search-user";
            _dbContext.Tasks.AddRange(
                new TaskItem { Title = "Buy groceries", UserId = userId },
                new TaskItem { Title = "Read book", UserId = userId }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskService.GetTasksForUserAsync(userId, null, "groceries");

            // Assert
            Assert.Single(result);
            Assert.Contains("groceries", result[0].Title);
        }

        [Fact]
        public async Task GetTasksForUserAsync_SearchDescription_ReturnsMatchingTasks()
        {
            // Arrange
            string userId = "search-user2";
            _dbContext.Tasks.AddRange(
                new TaskItem { Title = "Task 1", Description = "Meeting notes", UserId = userId },
                new TaskItem { Title = "Task 2", Description = "Home chores", UserId = userId }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskService.GetTasksForUserAsync(userId, "all", "chores");

            // Assert
            Assert.Single(result);
            Assert.Contains("chores", result[0].Description);
        }
        [Fact]
        public async Task GetTasksForUserAsync_SearchByKeyword_ReturnsMatchingTask()
        {
            // Arrange
            string userId = "user1";

            _dbContext.Tasks.AddRange(
                new TaskItem { Title = "Meeting", UserId = userId, Description = "Project discussion" },
                new TaskItem { Title = "Workout", UserId = userId, Description = "Health and fitness session" }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _taskService.GetTasksForUserAsync(userId, "all", "Health");

            // Assert
            Assert.Single(result);
            Assert.Contains("Health", result[0].Description);
        }



    }
}
