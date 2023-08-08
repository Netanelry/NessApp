var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Enable static file serving from wwwroot folder
app.UseStaticFiles();

app.MapGet("/", () =>
{
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");

    if (File.Exists(filePath))
    {
        return Results.Content(File.ReadAllText(filePath), "text/html");
    }
    else
    {
        return Results.Text("Hello World!");
    }
});

app.Run();