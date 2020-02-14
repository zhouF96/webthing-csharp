using System.Reflection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Factories.Generator.Intercepts;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    public class EmptyIntercept : IThingIntercept, IActionIntercept, IEventIntercept, IPropertyIntercept
    {
        public void Before(Thing thing)
        {
            
        }
        public void After(Thing thing)
        {
        }

        public void Visit(Thing thing, EventInfo @event, ThingEventAttribute? eventInfo)
        {
        }

        public void Intercept(Thing thing, MethodInfo action, ThingActionAttribute? actionInfo)
        {
            
        }
        
        public void Intercept(Thing thing, PropertyInfo propertyInfo, ThingPropertyAttribute? thingPropertyAttribute)
        {
            
        }
    }
}
