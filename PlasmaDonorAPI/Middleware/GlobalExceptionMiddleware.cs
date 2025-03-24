namespace NewPlasmaDonorsAPI.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500; // Internal Server Error
                var errorResponse = new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
            }
        }
    }

}
