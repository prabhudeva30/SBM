using Core.Services;
using WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the ThumbnailService with the dependency injection container
builder.Services.AddScoped<IThumbnailService, ThumbnailService>();

// Register CORS policy
var allowedHosts = builder.Configuration.GetSection("CORS:AllowedOrigins").Get<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedHostsPolicy", policy =>
    {
        policy.WithOrigins(allowedHosts!)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>(); // Custom middleware for exception handling

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowedHostsPolicy");
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();