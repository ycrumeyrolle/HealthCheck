using Microsoft.AspNetCore.Http;

namespace AspNetCore.HealthCheck
{
    public class ServerSwitchContext
    {
        public ServerSwitchContext(HttpContext context)
        {

            Context = context;
        }

        public HttpContext Context { get; }

        // TODO : Change properties name and methods
        public bool ServerDisabled { get; private set; }
       
        public int? RetryAfter { get; private set; }

        public string Message { get; private set; }

        public void Enable()
        {
            ServerDisabled = false;
        }

        public void Disable(int? retryAfter = null, string message = null)
        {
            ServerDisabled = true;
            RetryAfter = retryAfter;
            Message = message;
        }
    }
}