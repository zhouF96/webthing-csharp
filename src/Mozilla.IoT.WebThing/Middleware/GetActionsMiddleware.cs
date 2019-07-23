using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Description;

namespace Mozilla.IoT.WebThing.Middleware
{
    public class GetActionsMiddleware : AbstractThingMiddleware
    {
        public GetActionsMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IReadOnlyList<Thing> things)
            : base(next, loggerFactory.CreateLogger<GetActionsMiddleware>(), things)
        {
        }

        public async Task Invoke(HttpContext httpContext)
        {
            Thing thing = GetThing(httpContext);

            if (thing == null)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            var description = httpContext.RequestServices.GetService<IDescription<Action>>();


            var result = new LinkedList<IDictionary<string,object>>();

            foreach ((string name, ICollection<Action> actions) in thing.Actions)
            {
                foreach (Action action in actions)
                {
                    result.AddFirst(new Dictionary<string, object>
                    {
                        [name] = description.CreateDescription(action)
                    });
                }
            }

            await httpContext.WriteBodyAsync(HttpStatusCode.OK, result);
        }
    }
}
