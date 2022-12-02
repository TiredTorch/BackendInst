namespace WebApplication1
{
    public class BadStatusMiddleware
    {
		readonly RequestDelegate next;
		public BadStatusMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext ctx)
		{
			await next.Invoke(ctx);
			if (ctx.Response.StatusCode == 403)
			{
				await ctx.Response.WriteAsync("Access Denied");
			}
			else if (ctx.Response.StatusCode == 404) {
				await ctx.Response.WriteAsync($"{ctx.Request.Path} Not Found.");
			}
			else if (ctx.Response.StatusCode == 400)
			{
				await ctx.Response.WriteAsync($"Bad Request.");
			}
		}
	}
}
