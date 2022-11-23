using System.Globalization;

namespace WebApplication5
{
    public class BadRequestLogger : ILogger, IDisposable
    {
        string path;
        static object locker = new object();

        public BadRequestLogger(string path)
        {
            this.path = path;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            lock (locker)
            {
                File.AppendAllText(path, formatter(state, exception) + Environment.NewLine);
            }
        }
    }


    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly BadRequestLogger badRequestLogger;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            this.badRequestLogger = new BadRequestLogger("errors2.txt");
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                this.badRequestLogger.LogTrace(ex.StackTrace);
            }
            
        }
    }

}
