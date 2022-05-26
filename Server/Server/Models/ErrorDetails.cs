// https://code-maze.com/global-error-handling-aspnetcore/

using System.Text.Json;

namespace Server.Models
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
