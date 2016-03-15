using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Server {
    class Client {
        private Socket clientSocket;
        private Client[] clients;
        private int maxClientsCount;
        private string name;
        private StreamReader streamReader;
        private StreamWriter streamWriter;

        public void StartClient(Socket inClientSocket, Client[] clients) {
            clientSocket = inClientSocket;
            this.clients = clients;
            maxClientsCount = clients.Length;

            var ctThread = new Thread(DoChat);
            ctThread.Start();
        }

        private bool CheckCorrect(string s) {
            if (s.Equals("") || s.Equals("\n")) return false;
            return s.All(char.IsLetterOrDigit);
        }

        private static bool CheckCommand(string s) {
            return s.Equals("/list") || s.Equals("/quit") || s.Equals("") || s.Equals("\n");
        }

        private void DoChat() {
            int clientsCount = maxClientsCount;
            Client[] clients = this.clients;

            try {
                streamReader = new StreamReader(new NetworkStream(clientSocket));
                streamWriter = new StreamWriter(new NetworkStream(clientSocket));
                streamWriter.AutoFlush = true;
                string name;
                do {
                    streamWriter.WriteLine("*** Insert your name ***");
                    name = streamReader.ReadLine().Trim();
                    if (CheckCorrect(name)) break; 
                    streamWriter.WriteLine("*** Name cannot contains any special characters ***");
                } while (true);

                // Welcome
                Console.WriteLine("New user: " + name);
                streamWriter.WriteLine("*** Hello! " + name + " ***\n*** Write /quit to quit ***");
                streamWriter.WriteLine("*** Write /list to show other users ***");
                lock (this) {
                    for (int i = 0; i < clientsCount; i++) {
                        if (clients[i] != null && clients[i] == this) {
                            this.name = "@" + name;
                            break;
                        }
                    }
                    // Alert all users
                    for (int i = 0; i < clientsCount; i++) {
                        if (clients[i] != null && clients[i] != this) {
                            clients[i].streamWriter.WriteLine("*** " + name + " connected ***");
                        }
                    }
                }

                // Message checking
                while (true) {
                    var line = streamReader.ReadLine();
                    if (line != null && line.StartsWith("/quit")) break;     
                    if (line != null && line.StartsWith("/list")) 
                        foreach (Client client in clients.Where(client => client != null)) 
                            streamWriter.WriteLine(client.name);                      
                    else {
                        lock (this) {
                            if (!CheckCommand(line)) {
                                for (int i = 0; i < clientsCount; i++) 
                                    clients[i]?.streamWriter.WriteLine("<" + name + "> " + line);
                            }
                        }
                    }
                }

                // User disconnecting
                Console.WriteLine(name + " disconnected");
                lock (this) {
                    for (int i = 0; i < clientsCount; i++) {
                        if (clients[i] != null && clients[i] != null) {
                            clients[i].streamWriter.WriteLine("*** " + name + " disconnected ***");
                        }
                    }
                }

                lock (this) {
                    for (int i = 0; i < clientsCount; i++) 
                        if (clients[i] == this) clients[i] = null; 
                }
                streamReader.Close();
                streamWriter.Close();
                clientSocket.Close();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

        }
    }
}
