using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Primitives;
using System;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Xml.Linq;

namespace WebApplication5
{
    public static class SessionExtensions
    {
        public static void Set<T>(ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize<T>(value));
        }
        public static T? Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
        }
    }

    class MyClass
    {
        public string Time { get; set; } = "";
        public string SomeValue { get; set; } = "";
    }

    public class WebSiteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BadRequestLogger badRequestLogger;

        public WebSiteMiddleware(RequestDelegate next)
        {
            this.badRequestLogger = new BadRequestLogger("errors.txt");
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/err")
            {
                throw new Exception("some exeption");
            }
            if (context.Request.Path == "/")
            {
                var req = context.Request;
                var html_page = "";
                string? key = req.Query["key"];

                context.Response.ContentType = "text/html;charset=utf-8";

                if (key != "" && key != null) {
                    context.Request.Cookies.TryGetValue(key, out var result);
                    if (result != null)
                    {
                        html_page += $"There is this cookie!. key: {key} value: {result}";
                        this.badRequestLogger.LogInformation($"Success! key: {key}; value: {result}.");
                    }
                    else {
                        html_page += $"Cookie with key {key} doesnt exists.";
                        this.badRequestLogger.LogError($"Error! key {key} doesn't exists.");
                    }
                }
                
                html_page += @"<form action='/' method='get'>
                    <input type='text' name='key'>
                    <input type='submit'>
                </form>
                <a href='/form'>Add Cookie Form</a>";

                await context.Response.WriteAsync(html_page);
                return;
            }
            else if (context.Request.Path == "/form") {
                var html_page = "";
                
                if (context.Request.Method == "POST") {
                    var time = context.Request.Form["time"].ToString();
                    var key = context.Request.Form["key"].ToString();
                    var value = context.Request.Form["value"].ToString();

                    var dt = DateTime.ParseExact(time, "yyyy-MM-dd'T'HH:mm", CultureInfo.CurrentCulture);
                    context.Response.Cookies.Append(key, value, new CookieOptions
                    {
                        Expires = DateTime.SpecifyKind(dt, DateTimeKind.Utc),
                    });
                    html_page += "<h4>Cookie added!</h4><br>";
                    this.badRequestLogger.LogInformation($"Success! Added cookie! key: {key}; value: {value}; time:{time}.");
                }

                context.Response.ContentType = "text/html;charset=utf-8";
                html_page += @"<form action='form' method='post'>
                    <input type='text' name='key'>
                    <input type='text' name='value'>
                    <label for=''>Choose a time:</label>
                    <input type='datetime-local' id='time' name='time' value='2022-12-01T19:30'>
                    <input type='submit'>
                </form>
                <a href='/'>Check for a cookie</a>";
                await context.Response.WriteAsync(html_page);
                return;
            }

            await _next.Invoke(context);
        }
    }
}
