using System;
using System.Threading.Tasks;

namespace webapp
{
    public interface IPubSub
    {
        void Connect();
        Task ConnectAsync();
        void Subscribe(string topic, Action<string> action);
        Task SubscribeAsync(string topic, Action<string> action);
        void Publish(string topic, object message);
        Task PublishAsync(string topic, object message);
    }
}
