using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Porky.Classes
{
    public class Porky //TODO: need to setup the scan common tcp function. 
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

        public async Task SizzleTcp(string target)
        {
            List<int> openports = new List<int>();
            List<Task> tasks = new List<Task>();
            Dictionary<int,string> commonTcpPorts = new Dictionary<int, string> 
            {
                {21, "FTP" },
                {22, "SSH" },
                {23, "Telnet" },
                {25, "SMTP" },
                {53, "DNS" },
                {80, "HTTP" },
                {443, "HTTPS" },
                {445 , "SMB" },
                {1433, "MSSQL" },
                {3306, "MySQL" },
                {3389, "RDP" },
                {8080, "HTTP Proxy," },
            };

            Console.WriteLine($"Scanning {target} for Common TCP ports");   
            foreach (var TcpPorts in commonTcpPorts.Keys) 
            { 
                tasks.Add(ScanTcpAsync(target, TcpPorts, openports));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("\nScan complete! Open ports: ");

            foreach (var port in openports)
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
                        //await GrabbingBannerTcpAsync(client, port); Not working 
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
                if (stream.CanRead) 
                {
                    byte[] buffer = new byte[1024];
                    await stream.WriteAsync(Encoding.ASCII.GetBytes("\r\n"));
                    await Task.Delay(200);
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string banner = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (!string.IsNullOrWhiteSpace(banner))
                    {
                        Console.WriteLine($" Banner (TCP) {port}: {banner.Trim()}");
                    }
                }
               
            }
            catch(Exception ex) 
            {
                Console.WriteLine($" Banner (TCP {port}): {ex.Message}");
            }
        }
    }
}
