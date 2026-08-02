using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Pages
{
    public class TasksModel : PageModel
    {
        private readonly AppDbContext _db;
        public List<TaskItem> TaskItems { get; set; } = new();

        [BindProperty]
        public TaskItem NewTask { get; set; } = new();

        public TasksModel(AppDbContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
            TaskItems = await _db.TaskItems.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!string.IsNullOrWhiteSpace(NewTask.Title))
            {
                _db.TaskItems.Add(NewTask);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostToggleAsync(int id)
        {
            var task = await _db.TaskItems.FindAsync(id);
            if (task != null)
            {
                task.IsComplete = !task.IsComplete;
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
