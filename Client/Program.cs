// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    class Program
    {

        Socket? _socket;
        // int _port = 1010;
        private IPEndPoint _point;
        
        private Program()
        { 
            _point = new IPEndPoint(IPAddress.Loopback, 1024);
        }

        private void SendMessage(string? message)
        {

            _socket = new(
                SocketType.Stream,
                ProtocolType.Tcp);

            int counter = 0;
            _socket.Connect(_point);
            do
            {
                // Send message.
                var messageBytes = Encoding.UTF8.GetBytes(message);
                _socket?.Send(messageBytes, SocketFlags.None);
                Console.WriteLine($"Socket client sent message: \"{message}\"");

                counter++;
            } while (counter<1);

        }

        private void CloseConnection()
        {
            _socket?.Shutdown(SocketShutdown.Both);
            _socket?.Close();
            _socket = null;
        }

        private string GetServerResponse(Socket s)
        {
            byte[] fromServer = new byte[s.Available];
            Console.WriteLine(s.Available);
            s.Receive(fromServer);
            return Encoding.UTF8.GetString(fromServer);

        }

        public static void Main(String[] args)
        {


                Program client = new Program();
                client.SendMessage(Console.ReadLine());
                
                if (client._socket != null) Console.WriteLine(client.GetServerResponse(client._socket));
                
                client.CloseConnection();
        }

    }
}
    