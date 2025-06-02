using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Web.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title must be less than 100 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description must be less than 500 characters")]
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please select a category")]
        public TaskCategory Category { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        [CustomValidation(typeof(TaskItem), nameof(ValidateDueDate))]
        public DateTime? DueDate { get; set; }

        // Custom validation to prevent past due date as it is logical this way ^^
        public static ValidationResult? ValidateDueDate(DateTime? dueDate, ValidationContext context)
        {
            if (dueDate.HasValue && dueDate.Value.Date < DateTime.UtcNow.Date)
            {
                return new ValidationResult("Due date cannot be in the past");
            }
            return ValidationResult.Success;
        }
    }
}
