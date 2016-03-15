using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Server {
    class Server {
        private static TcpListener serverSocket;
        private static Socket clientSocket;
        private static int maxClientsCount = 3;
        private static readonly Client[] clients = new Client[maxClientsCount];

        private static void Main(string[] args) {

            serverSocket = new TcpListener(IPAddress.Any, 7777);
            serverSocket.Start();

            while (true) {
                Console.WriteLine("Establishing connection...");
                clientSocket = serverSocket.AcceptSocket();
                Console.WriteLine("Connected!");

                int i;
                for (i = 0; i < maxClientsCount; i++) {
                    if (clients[i] == null) {
                        (clients[i] = new Client()).StartClient(clientSocket, clients);
                        break;
                    }
                }

                if (i == maxClientsCount) {
                    StreamWriter streamWriter = new StreamWriter(new NetworkStream(clientSocket));
                    streamWriter.AutoFlush = true;
                    streamWriter.WriteLine("Error: server is full");
                    streamWriter.Close();
                    clientSocket.Close();
                }
                Console.WriteLine("ClientsNum: " + (i + 1));
            }
        }
    }
}
