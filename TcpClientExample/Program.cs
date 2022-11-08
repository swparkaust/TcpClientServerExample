using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace TcpClientExample
{
    public class Program
    {
        public static void Main()
        {
            IPAddress ipAddress = IPAddress.Loopback;

            // TODO: Uncomment the lines below to connect to a remote host.
            //IPHostEntry ipHostInfo = Dns.GetHostEntry("host.contoso.com");
            //ipAddress = ipHostInfo.AddressList[0];

            // Creates an IPEndPoint from a known IPAddress and port.
            var ipEndPoint = new IPEndPoint(ipAddress, 13);

            // Instantiate a new TcpClient object.
            TcpClient client = new TcpClient();
            // Connects the client to the remote TCP time server on port 13 using TcpClient.Connect.
            client.Connect(ipEndPoint);
            // Uses a NetworkStream to read data from the remote host.
            NetworkStream stream = client.GetStream();

            ThreadPool.QueueUserWorkItem(ThreadProc, stream);

            Console.Write("Enter your name: ");
            var userName = Console.ReadLine();

            var welcomeMessage = $"{userName} has entered the chat.";
            var welcomeMessageBytes = Encoding.UTF8.GetBytes(welcomeMessage);
            stream.Write(welcomeMessageBytes, 0, welcomeMessageBytes.Length);

            while (true)
            {
                Console.Write("> ");
                var message = Console.ReadLine();
                if (message == "/exit")
                {
                    break;
                }
                message = $"{userName}: {message}";
                var messageBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
            }

            client.Close();
        }

        private static void ThreadProc(object obj)
        {
            var stream = (NetworkStream)obj;
            try
            {
                // Declares a read buffer of 1_024 bytes.
                var buffer = new byte[1_024];
                // Reads data from the stream into the read buffer.
                int received;
                while ((received = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    // Writes the results as a string to the console.
                    var message = Encoding.UTF8.GetString(buffer, 0, received);
                    Console.WriteLine($"{message}");
                    // Sample output:
                    //     Message received: "📅 8/22/2022 9:07:17 AM 🕛"
                }
            }
            catch (IOException) { }
        }
    }
}
