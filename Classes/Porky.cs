using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Porky.Classes
{
    public class Porky
    {
        public Porky() {}

        public  async Task Sizzle(string target, int startPort, int endPort)
        {
            List<int> openPorts = new List<int>();
            List<Task> tasks = new List<Task>();

            Console.WriteLine($"Scanning {target} for open ports between {startPort}-{endPort}..");
            for (int port = startPort; port < endPort; port++) 
            {
                tasks.Add(ScanTcpAsync(target, port, openPorts));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("\nScan complete! Open ports: ");

            foreach (var port in openPorts) 
            {
                Console.WriteLine($"Port {port} - .....");
            }
        }

        public async Task ScanTcpAsync(string host, int port, List<int> openPorts) 
        {
            using (TcpClient client = new TcpClient())
            {
                try
                {
                    var connectTask = client.ConnectAsync(host, port);
                    if (await Task.WhenAny(connectTask, Task.Delay(500)) == connectTask)
                    {
                        Console.WriteLine($"[+] Open (TCP): {port}");
                        openPorts.Add(port);
                        await GrabbingBannerTcpAsync(client, port);
                    }
                }
                catch(Exception) { }
            }
        }

        public async Task GrabbingBannerTcpAsync(TcpClient client, int port)
        {
            try 
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string banner = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (!string.IsNullOrWhiteSpace(banner))
                {
                    Console.WriteLine($"Banner (TCP) {port}: {banner.Trim()}");
                }
            }
            catch(Exception) { }
        }
    }
}
