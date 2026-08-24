using AIAssistant.Api.Services;
using AIAssistant.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repository
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<OpenAIService>();
builder.Services.AddScoped<DeepSeekService>();
builder.Services.AddHttpClient<DeepSeekService>();
builder.Services.AddScoped<GoogleAIService>();
builder.Services.AddHttpClient<GoogleAIService>();
builder.Services.AddScoped<NvidiaService>();
builder.Services.AddHttpClient<NvidiaService>();

builder.Services.Configure<NvidiaNimOptions>(
    builder.Configuration.GetSection(NvidiaNimOptions.SectionName));

// Register using HttpClient typed client extension
//builder.Services.AddHttpClient<INvidiaAiService, NvidiaAiService>();
builder.Services.AddHttpClient<INvidiaAiService, NvidiaAiService>(client =>
{
    // Increase the timeout to 5 minutes (300 seconds)
    client.Timeout = TimeSpan.FromMinutes(5);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "API v1"));

    // Redirect root to Swagger UI
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger/index.html");
        return Task.CompletedTask;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
