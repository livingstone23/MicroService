

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace HNL.MessageBus
{
    public class AzureServiceBusMessageBus : IMessageBus
    {
        //can be improved
        private string connectionString = "Endpoint=sb://mangorestaurant.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=7t3usJ6tooj30PRi5ccrTwbxS6DNGPxbmp2oVdiO3cI=";

        public async Task PublishMessage(BaseMessage message, string topicName)
        {
            //Primera version del metodo con paquete Microsoft.Azure.Servicebus 5.2.0
            ISenderClient senderClient = new TopicClient(connectionString, topicName);

                var jsonMessage = JsonConvert.SerializeObject(message);

                var finalMessage = new Message(Encoding.UTF8.GetBytes(jsonMessage))
                                    {
                                        CorrelationId = Guid.NewGuid().ToString()
                                    };

                await senderClient.SendAsync(finalMessage);

                await senderClient.CloseAsync();



            //await using var client = new ServiceBusClient(connectionString);

            //ServiceBusSender sender = client.CreateSender(topicName);

            //var jsonMessage = JsonConvert.SerializeObject(message);
            //ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            //{
            //    CorrelationId = Guid.NewGuid().ToString()
            //};

            //await sender.SendMessageAsync(finalMessage);

            //await client.DisposeAsync();
        }
    }
}
