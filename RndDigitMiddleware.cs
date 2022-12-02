namespace WebApplication1
{
    public class RndDigitMiddleware
    {
		readonly RequestDelegate next;
		public RndDigitMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext ctx)
		{
			if (ctx.Request.Path == "/Random") {
				int rnd = new Random().Next(100);
				await ctx.Response.WriteAsync($"Result: {rnd}");
			} else {
				await next.Invoke(ctx);
			}
		}
	}
}
