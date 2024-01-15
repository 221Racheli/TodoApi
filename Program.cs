using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoApi;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Item>();
builder.Services.AddCors();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ToDoDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));
});


var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseCors(builder => builder
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader()
);

app.MapGet("/", () => "welcome to your to do list!");

app.MapGet("/task", (ToDoDbContext dbContext) => dbContext.Items.ToList());

var AddItem = async (ToDoDbContext dbContext, [FromBody] Item item) =>
{
    await dbContext.Items.AddAsync(item);
    await dbContext.SaveChangesAsync();
    return item;
};

app.MapPost("/task", AddItem);

var UpdateAddItem = async (ToDoDbContext dbContext, [FromBody] Item item) =>
{
    dbContext.Items.Update(item);
    await dbContext.SaveChangesAsync();
    return item;
};

app.MapPut("/task", UpdateAddItem);

var DeleteItem = async (ToDoDbContext dbContext, int id) =>
{
    var item = dbContext.Items.Find(id);
    dbContext.Items.Remove(item);
    await dbContext.SaveChangesAsync();
    return item;
};

app.MapDelete("/task/{id}", DeleteItem);


app.MapMethods("/options-or-head", new[] { "OPTIONS", "HEAD" },
                          () => "This is an options or head request ");

app.Run();



