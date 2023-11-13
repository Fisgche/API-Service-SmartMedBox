using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace CaixaDeRemedios
{
    public class MQTT
    {
        private MqttClient client;

        public event Action<string, string> MessageReceived;

        public MQTT(string endereco) {

            client = new MqttClient(endereco);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }
        public void Connect()
        {
            client.Connect(Guid.NewGuid().ToString());
        }

        public void Subscribe(string topic)
        {
            client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string topic = e.Topic;
            string message = System.Text.Encoding.UTF8.GetString(e.Message);

            // Dispare o evento de mensagem recebida
            MessageReceived?.Invoke(topic, message);
        }
    }
}
