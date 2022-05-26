// https://code-maze.com/global-error-handling-aspnetcore/

using Server.CustomExceptionMiddleware;

namespace Server.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
