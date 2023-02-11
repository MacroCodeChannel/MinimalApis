using Microsoft.EntityFrameworkCore;
using MinimalApi.Data;
using MinimalApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


async Task<List<Student>> GetStudents(DataContext context) => await context.Students.ToListAsync();


app.MapPost("Add/Student", async (DataContext context, Student item) =>
{
    context.Students.Add(item);
    await context.SaveChangesAsync();
    return Results.Ok(await GetStudents(context));
})
.WithName("AddStudent")
.WithOpenApi();



app.MapGet("/Student", async (DataContext context) => 
await context.Students.ToListAsync())
.WithName("GetStudent")
.WithOpenApi();


app.MapGet("/Student/{id}", async (DataContext context,int id) => await context.Students.FindAsync(id) is Student item? Results.Ok(item): Results.NotFound("Student not found"))
.WithName("GetStudentById")
.WithOpenApi();

app.MapPut("/Student/{id}", async (DataContext context, Student item,int id) =>
{

    var studentitem = await context.Students.FindAsync(id);
    if (studentitem == null) return Results.NotFound("Student not Found");
    studentitem.FirstName = item.FirstName;
    studentitem.LastName = item.LastName;
    context.Students.Update(studentitem);
    await context.SaveChangesAsync();
    return Results.Ok(await GetStudents(context));
})
.WithName("UpdateStudentById")
.WithOpenApi();


app.MapDelete("/Student/{id}", async (DataContext context, int id) =>
{

    var studentitem = await context.Students.FindAsync(id);
    if (studentitem == null) return Results.NotFound("Student not Found");
    context.Remove(studentitem);
    await context.SaveChangesAsync();
    return Results.Ok(await GetStudents(context));
})
.WithName("DeleteStudentById")
.WithOpenApi();


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
