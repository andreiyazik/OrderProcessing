using Newtonsoft.Json.Linq;

namespace OrderProcessing.Domain.Events;

public class ServiceBusMessage
{
    public string Name { get; set; }
    public JObject Payload { get; set; }
}
