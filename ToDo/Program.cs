using ToDo.Repository;
using ToDo.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();

var app = builder.Build();


app.UseHttpsRedirection();
app.MapControllers();

app.Run();
