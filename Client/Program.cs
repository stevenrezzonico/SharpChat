using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client {
    class Program {
        private static TcpClient client;
        private static StreamReader streamReader;
        private static StreamWriter streamWriter;


        static void Main(string[] args) {
            try {
                client = new TcpClient("127.0.0.1", 7777);
                streamReader = new StreamReader(client.GetStream());
                streamWriter = new StreamWriter(client.GetStream());
                streamWriter.AutoFlush = true;
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            if (client != null && streamWriter != null && streamReader != null) {
                try {
                    ClientThread cli = new ClientThread(client, streamReader, streamWriter);
                    Thread ctThread = new Thread(cli.run);
                    ctThread.Start();

                    while (!cli.closed) {
                        string msg = Console.ReadLine().Trim();
                        streamWriter.WriteLine(msg);
                    }
                    streamWriter.Close();
                    streamReader.Close();
                    client.Close();
                }
                catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
