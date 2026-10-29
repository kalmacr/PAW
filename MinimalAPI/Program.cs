//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast")
//.WithOpenApi();

//app.Run();

//internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
using PAW.MinimalAPI.Services;
using PAW.Models.DTO;
using ModelTask = PAW.Models.Task;

var builder = WebApplication.CreateBuilder(args);

// Inyección de dependencias (DI/IoC) sin DbContext
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints Minimal API utilizando Tasks y DTOs
app.MapGet("/task", async (ITaskRepository repo) =>
{
    var tasks = await repo.GetAllAsync();
    var taskDtos = tasks.Select(TaskDTO.ConvertFrom);
    return Results.Ok(taskDtos);
});

app.MapGet("/task/{id:int}", async (int id, ITaskRepository repo) =>
{
    var task = await repo.GetByIdAsync(id);
    return task is not null ? Results.Ok(TaskDTO.ConvertFrom(task)) : Results.NotFound();
});

app.MapPost("/task", async (TaskDTO taskDto, ITaskRepository repo) =>
{
    var entity = TaskDTO.Convert(taskDto);
    await repo.SaveAsync(entity);
    return Results.Ok(true);
});

app.MapDelete("/task/{id:int}", async (int id, ITaskRepository repo) =>
{
    var success = await repo.DeleteAsync(id);
    return success ? Results.Ok(true) : Results.NotFound();
});

app.Run();