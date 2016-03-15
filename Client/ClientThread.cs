using System;
using System.IO;
using System.Net.Sockets;

namespace Client {
    class ClientThread {
        public bool closed = false;
        private TcpClient client;
        private StreamReader streamReader;
        private StreamWriter ots;

        public ClientThread(TcpClient client, StreamReader streamReader, StreamWriter ots) {
            this.client = client;
            this.streamReader = streamReader;
            this.ots = ots;
        }

        public void run() {
            string responseLine;
            try {
                while ((responseLine = streamReader.ReadLine()) != null) {
                    Console.WriteLine(responseLine);
                    if (responseLine.IndexOf("*** Adios") != -1) {
                        break;
                    }
                }
                closed = true;
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
            Environment.Exit(0);
        }
    }
}
