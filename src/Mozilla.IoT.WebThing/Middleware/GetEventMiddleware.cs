using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetEventMiddleware : AbstractThingMiddleware
    {
        public GetEventMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things) 
            : base(next, loggerFactory.CreateLogger<GetEventMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (thing == null)
            {
                httpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return;
            }
            
            var descriptor = httpContext.RequestServices.GetService<IDescription<Event>>();

            string eventName = httpContext.GetValueFromRoute<string>("eventName");
            
            var result = thing.Events
                .Where(x => x.Name == eventName)
                .ToDictionary<Event, string, object>(@event => @event.Name,
                    @event => descriptor.CreateDescription(@event));
            
            await httpContext.WriteBodyAsync(HttpStatusCode.OK,
                    result);
        }
    }
}
