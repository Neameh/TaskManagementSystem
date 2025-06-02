using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using TaskManagementSystem.Web.Exceptions;
using TaskManagementSystem.Web.Models;
using TaskManagementSystem.Web.Services;

namespace TaskManagementSystem.Web.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {

        private readonly ITaskService _taskService;
        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        public async Task<IActionResult> Index(string filter = "all", string search="")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NotFoundException("User Not Found");
            var tasks = await _taskService.GetTasksForUserAsync(userId,filter,search);
            ViewBag.CurrentFilter = filter;
            ViewBag.Search = search;
            return View(tasks);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskItem model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NotFoundException("User Not Found");
                await _taskService.CreateTaskAsync(model);
                TempData["Success"] = "Task created!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NotFoundException("User Not Found");
            var task = await _taskService.GetTaskByIdAsync(id.Value, userId);
            if (task == null)
                return NotFound();

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskItem model) // UserId??
        {
            if (id != model.Id)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NotFoundException("User Not Found");
            model.UserId = userId;
            if (ModelState.IsValid)
            {
                await _taskService.UpdateTaskAsync(model);
                TempData["Success"] = "Task updated!";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NotFoundException("User Not Found");
            TempData["Success"] = "Task Deleted!";
            await _taskService.DeleteTaskAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleComplete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new NotFoundException("User Not Found");
            await _taskService.ToggleCompleteAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
