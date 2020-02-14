namespace Mozilla.IoT.WebThing.WebSockets
{
    public class WebSocketResponse
    {
        public WebSocketResponse(string messageType, object data)
        {
            MessageType = messageType;
            Data = data;
        }

        public string MessageType { get; }
        public object Data { get; }
    }

    public class ErrorResponse
    {
        public ErrorResponse(string status, string message)
        {
            Status = status;
            Message = message;
        }

        public string Status { get; }
        public string Message { get; }
    }
}
