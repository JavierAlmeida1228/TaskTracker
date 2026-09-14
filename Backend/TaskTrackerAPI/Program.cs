using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using TaskTrackerAPI.Data;
using TaskTrackerAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Register SQLite database
builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Endpoint to fetch tasks from DB
app.MapGet("/tasks", async (TaskContext db) =>
    await db.Tasks.ToListAsync());

app.Run();
