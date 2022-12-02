namespace WebApplication1
{
	public class AuthMiddleware
	{
		readonly RequestDelegate next;
		public AuthMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext ctx)
		{
			var token = ctx.Request.Query["token"];
            if (token != "12345678")
            {
                ctx.Response.StatusCode = 403;
                await ctx.Response.WriteAsync("Token is invalid\n");
            }
            else
            {
                await next.Invoke(ctx);
            }
        }
	}
}
