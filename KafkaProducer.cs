using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Target_Receiver
{
    public static class KafkaProducer
    {
        private static string topicName;
        public static ProducerConfig conf = new ProducerConfig
        {
            BootstrapServers = ConfigFile.KAFKA,//this.bootStrapServer,
            QueueBufferingMaxKbytes = 2097152,
            QueueBufferingMaxMessages = 100000,
            LingerMs = 5,
            EnableBackgroundPoll = true,
        };


        private static IProducer<Null, byte[]> p = new ProducerBuilder<Null, byte[]>(conf).Build();
        //private string topicName;
        //private string bootStrapServer;
        //private byte[] protobuf;

        //public KafkaProducer(string topicName,byte[] protobuf, string bootStrapServer = "localhost:9092")
        //{
        //    this.topicName = "tp_"  + topicName;
        //    this.protobuf = protobuf;
        //    this.bootStrapServer = bootStrapServer;

        //}   

        public static void Send(string pTopicName, byte[] protobuf)
        {

            var pollvalue = conf.EnableBackgroundPoll;

            topicName = "tp_" + pTopicName;
            int? max = conf.MessageMaxBytes;

            Action<DeliveryReport<Null, byte[]>> handler = r =>
            {
                if (!r.Error.IsError)
                {
                    SendOK(r);
                }
                else
                {
                    SendError(r);
                }
            };
            p.Produce(topicName, new Message<Null, byte[]> { Value = protobuf }, handler);

        }

        private static void SendError(DeliveryReport<Null, byte[]> r)
        {
            Console.WriteLine(r.Error.Reason); ;
        }

        private static void SendOK(DeliveryReport<Null, byte[]> r)
        {
            //Console.WriteLine(r.TopicPartitionOffset);
        }
    }
}
