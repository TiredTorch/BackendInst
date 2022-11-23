using WebApplication5;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

app.UseSession();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<WebSiteMiddleware>();
//app.UseMiddleware<LogBadRequestsMiddleware>();

app.Use(async (context, next) =>
{
    await next.Invoke();
    if (context.Response.StatusCode == 404)
        await context.Response.WriteAsync("Page not found!");
});

app.Run();