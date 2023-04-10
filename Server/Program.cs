// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{

    class Program
    {
        
        delegate void ConnectDelegate (Socket s);
        delegate void StartNetwork(Socket s);
        
        Socket? socket;
        IPEndPoint endP;
        private List<string> messages = new List<string>();

        public Program(string strAddr, int port)
        {
            endP = new IPEndPoint(IPAddress.
                Parse(strAddr), port);
        }
        
        
        void Server_Connect(Socket s)
        {
            s.Send(Encoding.ASCII.
                GetBytes(DateTime.UtcNow.ToString()));
            
            s.Shutdown(SocketShutdown.Both);
            s.Close();
        }
        
        void Server_Begin(Socket s)
        {
            int i = 0;
            do
            {
                try
                {
                    while (s != null)
                    {
                        Socket ns = s.Accept();
                        Console.WriteLine(
                            "request IP and port" +
                            ns.RemoteEndPoint);
                        Console.WriteLine("Available data: " + ns.Available);
                        byte[] responseData = new byte[ns.Available];

                        ns.Receive(responseData);

                        string requestMessage = Encoding.UTF8.GetString(responseData);

                        Console.WriteLine("Received message:" + requestMessage);
                        messages.Add(requestMessage);

                        Server_Connect(ns);

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                i++;
            } while (i < 1);
        }
        
        private void Start()
        {
            if (socket != null)
            {
                return;
            }

            socket = new Socket(SocketType.Stream,
                ProtocolType.Tcp);
            socket.Bind(endP);
            socket.Listen(10);
            StartNetwork start = Server_Begin;
            start.Invoke(socket);
        }
        
        private void Stop()
        {
            if (socket != null)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    socket = null;
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Environment.Exit(0);
            
        }

        public static void Main(String[] args) 
        {

            
            Program s = new Program("127.0.0.1", 1024);
            s.Start();
            s.Stop();
            
            
        }
    }
}