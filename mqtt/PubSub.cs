using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace webapp.mqtt
{
    public class PubSub : IPubSub
    {
        IMqttClient client;
        IMqttClientOptions options;
        private PubSubConfig _config;
        Dictionary<string, Action<string>> actions = new Dictionary<string, Action<string>>();
            
        public PubSub(IOptions<PubSubConfig> options){
            _config = options.Value;
        }
        
        public void Connect()
        {
            options = new MqttClientOptionsBuilder()
                .WithClientId(_config.ClientId)
                .WithTcpServer(_config.Host)
                .WithCleanSession()
                .Build();

            client = new MQTTnet.MqttFactory().CreateMqttClient();
            client.UseApplicationMessageReceivedHandler(e => { HandleMessageReceived(e.ApplicationMessage); });
            client.UseConnectedHandler(e => Console.WriteLine("### CONNECTED WITH BROKER ###"));

            client.ConnectAsync(options, CancellationToken.None).Wait();
        }

        public async Task ConnectAsync() { 

            options = new MqttClientOptionsBuilder()
                .WithClientId(_config.ClientId)
                .WithTcpServer(_config.Host)
                .WithCleanSession()
                .Build();

            client = new MQTTnet.MqttFactory().CreateMqttClient();
            client.UseApplicationMessageReceivedHandler(e => { HandleMessageReceived(e.ApplicationMessage); });
            client.UseConnectedHandler(e => Console.WriteLine("### CONNECTED WITH BROKER ###"));

            await client.ConnectAsync(options, CancellationToken.None);
        }

        public void Subscribe(string topic, Action<string> action)
        {
            if (client == null || !client.IsConnected) Connect();
            client.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(topic)
                    .Build());
            
            actions[topic] = action;
        }

        public void Publish(string topic, object message)
        {
            if (client == null || !client.IsConnected) Connect();
            
            client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithPayload(Encoding.UTF8.GetBytes(message is string ? (string)message : JsonSerializer.Serialize(message)))
                .WithTopic(topic)
                .Build(), CancellationToken.None);
        }

        public async Task SubscribeAsync(string topic, Action<string> action)
        {
            if (client == null || !client.IsConnected) await ConnectAsync();
            await client.SubscribeAsync(new MqttTopicFilterBuilder()
                    .WithTopic(topic)
                    .Build());
            
            actions[topic] = action;
        }

        public async Task PublishAsync(string topic, object message)
        {
            if (client == null || !client.IsConnected) await ConnectAsync();

            await client.PublishAsync(new MqttApplicationMessageBuilder()
                .WithPayload(Encoding.UTF8.GetBytes(message is string ? (string)message : JsonSerializer.Serialize(message)))
                .WithTopic(topic)
                .Build(), CancellationToken.None);
        }

        private void HandleMessageReceived(MqttApplicationMessage applicationMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(applicationMessage.Topic) == false)
                    if (actions.ContainsKey(applicationMessage.Topic))
                        actions[applicationMessage.Topic](Encoding.UTF8.GetString(applicationMessage.Payload));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
        }
    }
}
