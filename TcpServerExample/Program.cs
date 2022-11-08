using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace TcpServerExample
{
    public class Program
    {
        private static List<TcpClient> clients = new List<TcpClient>();

        public static void Main()
        {
            // Creates an IPEndPoint with IPAddress.Any and port.
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 13);
            // Instantiate a new TcpListener object.
            TcpListener listener = new TcpListener(ipEndPoint);

            try
            {
                // Calls the Start method to start listening on the port.
                listener.Start();

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Uses a TcpClient from the AcceptTcpClient method to accept incoming connection requests.
                    TcpClient handler = listener.AcceptTcpClient();
                    clients.Add(handler);
                    ThreadPool.QueueUserWorkItem(ThreadProc, handler);
                }
            }
            finally
            {
                // Finally, calls the Stop method to stop listening on the port.
                listener.Stop();

                Console.WriteLine("\nPress ENTER to continue...");
                Console.Read();
            }
        }

        private static void ThreadProc(object obj)
        {
            var handler = (TcpClient)obj;
            try
            {
                NetworkStream stream = handler.GetStream();

                // Declares a read buffer of 1_024 bytes.
                var buffer = new byte[1_024];

                // Reads data from the stream into the read buffer.
                int received;
                while ((received = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    foreach (TcpClient client in clients)
                    {
                        // Uses a NetworkStream to write data to the connected clients.
                        client.GetStream().Write(buffer, 0, received);
                    }
                }
            }
            catch (Exception)
            {
                handler.Close();
                clients.Remove(handler);
            }
        }
    }
}
