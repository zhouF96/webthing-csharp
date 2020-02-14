using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.AcceptanceTest.WebScokets
{
    public class Event
    {
        [Theory]
        [InlineData("overheated")]
        public async Task EventSubscription(string @event)
        {
            var host = await Program.CreateHostBuilder(null)
                .StartAsync()
                .ConfigureAwait(false);
            var client = host.GetTestServer().CreateClient();
            var webSocketClient = host.GetTestServer().CreateWebSocketClient();

            var uri =  new UriBuilder(client.BaseAddress)
            {
                Scheme = "ws",
                Path = "/things/lamp"
            }.Uri;
            var socket = await webSocketClient.ConnectAsync(uri, CancellationToken.None);

            await socket
                .SendAsync(Encoding.UTF8.GetBytes($@"
{{
    ""messageType"": ""addEventSubscription"",
    ""data"": {{
        ""{@event}"": {{}}
    }}
}}"), WebSocketMessageType.Text, true,
                    CancellationToken.None)
                .ConfigureAwait(false);
            
            var segment = new ArraySegment<byte>(new byte[4096]);
            var result = await socket.ReceiveAsync(segment, CancellationToken.None)
                .ConfigureAwait(false);

            result.MessageType.Should().Be(WebSocketMessageType.Text);
            result.EndOfMessage.Should().BeTrue();
            result.CloseStatus.Should().BeNull();

            var json = JToken.Parse(Encoding.UTF8.GetString(segment.Slice(0, result.Count)));
            json.Type.Should().Be(JTokenType.Object);

            var obj = (JObject)json;
            
            obj.GetValue("messageType", StringComparison.OrdinalIgnoreCase).Type.Should()
                .Be(JTokenType.String);
            obj.GetValue("messageType", StringComparison.OrdinalIgnoreCase).Value<string>().Should()
                .Be("event");

            ((JObject)obj.GetValue("data", StringComparison.OrdinalIgnoreCase))
                .GetValue("overheated", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Object);


            var overheated = ((JObject)((JObject)obj.GetValue("data", StringComparison.OrdinalIgnoreCase))
                .GetValue("overheated", StringComparison.OrdinalIgnoreCase));
            
            overheated
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Integer);
            
            overheated
                .GetValue("timestamp", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Date);
            
            var response = await client.GetAsync("/things/Lamp/events/overheated");
            
            response.IsSuccessStatusCode.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType.ToString().Should().Be( "application/json");
            
            var message = await response.Content.ReadAsStringAsync();
            json = JToken.Parse(message);
            
            json.Type.Should().Be(JTokenType.Array);
            ((JArray)json).Should().HaveCountGreaterOrEqualTo(1);

            obj = ((JArray)json)[0] as JObject;
            obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Object);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Integer);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("data", StringComparison.OrdinalIgnoreCase).Value<int>().Should().Be(0);
            
            ((JObject)obj.GetValue("overheated", StringComparison.OrdinalIgnoreCase))
                .GetValue("timestamp", StringComparison.OrdinalIgnoreCase).Type.Should().Be(JTokenType.Date);

        }
    }
}
