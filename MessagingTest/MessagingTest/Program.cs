using System;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace MessagingTest
{
    public class MyMessage
    {
        public string Command { get; set; }
        public int EntityID { get; set; }
    }

    class Program
    {
        private const string MessageAddress = @".\Private$\MyProgram";
        private const string QueueName = ".\\Private$\\MyProgram";

        private static void SendMessage(string str, int x)
        {
            using (var msmq = new MessageQueue(MessageAddress))
                msmq.Send(new MyMessage
                {
                    Command = str,
                    EntityID = x
                });
        }

        static void Main(string[] args)
        {
            try
            {
                if (!MessageQueue.Exists(QueueName))
                {
                    MessageQueue.Create(QueueName);
                }

                var t = new Task(() =>
                {
                    var i = 0;
                    while (true)
                    {
                        SendMessage("hello", i);
                        Console.WriteLine("Message sent. Message number: " + i);
                        i++;
                        Thread.Sleep(1500);
                    }
                });

                t.Start();

                Console.ReadKey();
            }
            finally
            {
                MessageQueue.Delete(QueueName);
            }
        }
    }
}
