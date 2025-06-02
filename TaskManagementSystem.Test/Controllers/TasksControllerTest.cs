using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagementSystem.Web.Controllers;
using TaskManagementSystem.Web.Models;
using TaskManagementSystem.Web.Services;
using Xunit;

namespace TaskManagementSystem.Test.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskService> _taskServiceMock;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _taskServiceMock = new Mock<ITaskService>();
            _controller = new TasksController(_taskServiceMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user1")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Index_ReturnsViewWithTasks()
        {
            // Arrange
            var userId = "user1";
            var expectedTasks = new List<TaskItem>
                    {
                        new TaskItem { Title = "Task 1", UserId = userId }
                     };

            _taskServiceMock
                .Setup(s => s.GetTasksForUserAsync(userId, "all", ""))
                .ReturnsAsync(expectedTasks);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
                  {
                   new Claim(ClaimTypes.NameIdentifier, userId)
                }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);
            var model = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(viewResult.Model);
            Assert.Single(model);
        }


        [Fact]
        public async Task Create_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var mockService = new Mock<ITaskService>();
            var controller = new TasksController(mockService.Object);

            var task = new TaskItem { Title = "Test", Description = "Test Description" };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, "user1")
    }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Setup TempData manually (required for TempData["Success"])
            var tempData = new TempDataDictionary(controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            mockService.Setup(s => s.CreateTaskAsync(It.IsAny<TaskItem>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await controller.Create(task);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            mockService.Verify(s => s.CreateTaskAsync(It.IsAny<TaskItem>()), Times.Once);
        }



        [Fact]
        public async Task Edit_Get_ValidId_ReturnsView()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Title = "Edit Me", UserId = "user1" };
            _taskServiceMock.Setup(s => s.GetTaskByIdAsync(1, "user1")).ReturnsAsync(task);

            // Act
            var result = await _controller.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(task, viewResult.Model);
        }

        [Fact]
        public async Task Delete_Post_DeletesAndRedirects()
        {
            // Arrange
            var userId = "user1";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId)
    }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var tempData = new TempDataDictionary(_controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;

            _taskServiceMock
                .Setup(s => s.DeleteTaskAsync(1, userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            _taskServiceMock.Verify(s => s.DeleteTaskAsync(1, userId), Times.Once);
        }



        [Fact]
        public async Task Edit_Post_ValidModel_UpdatesAndRedirects()
        {
            // Arrange
            var task = new TaskItem { Id = 1, Title = "Updated Task", UserId = "user1" };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim(ClaimTypes.NameIdentifier, "user1")
    }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _controller.TempData = new TempDataDictionary(
                _controller.ControllerContext.HttpContext,
                Mock.Of<ITempDataProvider>()
            );

            // Act
            var result = await _controller.Edit(1, task);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            _taskServiceMock.Verify(s => s.UpdateTaskAsync(task), Times.Once);
        }

        [Fact]
        public async Task Edit_Post_MismatchedId_ReturnsNotFound()
        {
            // Arrange
            var task = new TaskItem { Id = 2, Title = "Wrong ID", UserId = "user1" };

            // Act
            var result = await _controller.Edit(1, task);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
        [Fact]
        public async Task ToggleComplete_ValidId_TogglesAndRedirects()
        {
            // Act
            var result = await _controller.ToggleComplete(1);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            _taskServiceMock.Verify(s => s.ToggleCompleteAsync(1, "user1"), Times.Once);
        }


    }
}
