using System;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace MessageTestReceiver
{
    public class Program
    {
        private const string QueueName = ".\\Private$\\MyProgram";

        public class MyMessage
        {
            public string Command { get; set; }
            public int EntityID { get; set; }
        }

        private static MyMessage RecieveMessage()
        {
            if (!MessageQueue.Exists(QueueName))
            {
                return null;
            }

            using (var msmq = new MessageQueue(QueueName))
            {
                msmq.Formatter = new XmlMessageFormatter(new Type[] { typeof(MyMessage) });
                var message = msmq.Receive();
                return message != null && message.Body is MyMessage ? (MyMessage)message.Body : null;
            }
        }

        static void Main(string[] args)
        {
            var t2 = new Task(() =>
            {
                while (true)
                {
                    var msg = RecieveMessage();
                    while (msg != null)
                    {
                        Console.WriteLine("Message received: " + msg.Command + " " + msg.EntityID);
                        msg = RecieveMessage();
                    }

                    Thread.Sleep(1000);
                }
            });

            t2.Start();

            Console.ReadKey();
        }
    }
}
