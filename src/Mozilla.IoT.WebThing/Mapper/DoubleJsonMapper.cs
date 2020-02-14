using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class DoubleJsonMapper : IJsonMapper
    {
        private static DoubleJsonMapper? s_instance;
        public static DoubleJsonMapper Instance => s_instance ??= new DoubleJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetDouble();
    }
}
