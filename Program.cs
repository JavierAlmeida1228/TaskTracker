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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();
app.MapGet("/", context =>
{
    context.Response.Redirect("/Tasks");
    return Task.CompletedTask;
});




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
