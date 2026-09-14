using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Remove HTTPS redirection for Docker
// app.UseHttpsRedirection();

app.UseStaticFiles();        // Required for Razor Pages
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

// Redirect root → /Tasks (assignment requirement)
app.MapGet("/", context =>
{
    context.Response.Redirect("/Tasks");
    return Task.CompletedTask;
});

// Minimal API endpoints (assignment requirement)
app.MapGet("/api/tasks", async (AppDbContext db) =>
    await db.TaskItems.ToListAsync());

app.MapPost("/api/tasks", async (AppDbContext db, TaskItem task) =>
{
    db.TaskItems.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{task.TaskItemId}", task);
});

app.MapPatch("/api/tasks/{id}/complete", async (AppDbContext db, int id) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.IsComplete = !task.IsComplete;
    await db.SaveChangesAsync();
    return Results.Ok(task);
});

app.MapDelete("/api/tasks/{id}", async (AppDbContext db, int id) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.TaskItems.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
