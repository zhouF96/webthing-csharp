using System.Text.Json;

namespace Mozilla.IoT.WebThing.Mapper
{
    public class DateTimeJsonMapper : IJsonMapper
    {
        private static DateTimeJsonMapper? s_instance;
        public static DateTimeJsonMapper Instance => s_instance ??= new DateTimeJsonMapper();

        public object Map(object value) 
            => ((JsonElement)value).GetDateTime();
    }
}
