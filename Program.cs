﻿using System;
using System.Threading.Tasks;
using Porky.Classes;

namespace Porky 
{
    class Program
    {
        ///dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o output
        static async Task Main(string[] args)
        { 
            string target = "";
            int startPort = 0; 
            int endPort = 0;
            bool scanAllPorts = false;
            bool tcpCommonPorts = false;

            Classes.Porky pork = new Classes.Porky();

            for (int i = 0; i < args.Length; i++) //Collecting parameters
            {
                switch (args[i])
                {
                    case "-ip":
                        target = args[++i]; break;
                    case "-p":
                        string[] ports = args[++i].Split('-');
                        startPort = int.Parse(ports[0]);
                        endPort = ports.Length > 1 ? int.Parse(ports[1]) : startPort; break;
                    case "-all":
                        scanAllPorts = true; break;
                    case "-tcp":
                        tcpCommonPorts = true; break;
                }
            }
           
            if (String.IsNullOrEmpty(target)) // Requesting parameters
            {
                Console.Write("Please Enter Ip or Hostname: ");
                target = Console.ReadLine();
            }

            if (!scanAllPorts && !tcpCommonPorts)
            {
                if (startPort == 0 || endPort == 0)
                {
                    Console.Write("Enter starting Port: ");
                    startPort = int.Parse(Console.ReadLine());

                    Console.Write("Enter ending Port: ");
                    endPort = int.Parse(Console.ReadLine());
                }
            }
            else
            {
                startPort = 1;
                endPort = 65535;
                
            }


            if (tcpCommonPorts == true)
            {
                await pork.SizzleTcp(target);
            }
            else
            {
                await pork.Sizzle(target, startPort, endPort);
            }
           
            // might need to create identifyservice class to handle that function.
        }
    }
}


