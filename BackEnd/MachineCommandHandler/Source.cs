using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Pololu.Usc;
using Pololu.UsbWrapper;

using Grpc.Core; // 
using MachineCommandServiceProtoBuf; // The compiled proto file

using static MachineCommandHandler.Repository; // used to share the data between classes
using System.Threading;

namespace MachineCommandHandler
{
    class Source
    {
        //static List<Maestro> maestros = new List<Maestro>();
        static DeviceListItem referenceMaestro, referenceMotor;

        const int port = 50051;

        static void Main(string[] args)
        {
            #region Debug Code
            #if DEBUG
            machines = new List<Machine>();
            testServos = new List<Servo>();
            
            for (int i = 0; i < 100; i++)
            {
                machines.Add(new ClawMachine(current_machine_id.ToString()));
                machines.Add(new SkeeBall((current_machine_id + 1).ToString()));
                current_machine_id += 2;
            }

            /* This code had originally been used for testing concurrent connections to my servos/controllers
            foreach (DeviceListItem d in Usc.getConnectedDevices())
            {
                if(d.serialNumber == "00216679")
                {
                    referenceMaestro = d;
                    Maestro testMaeastro = new Maestro(new Usc(d), d.serialNumber); // quick fix - change later
                    testServo = new Servo(testMaeastro, 0);
                    foreach (int i in testMaeastro.ServosConnected())
                        testServos.Add(new Servo(testMaeastro, i));
                }
            }

            testMotor = new TicMotor("00254354");*/
            #endif
            #endregion

            Server server = new Server
            {
                Services = { MachineCommandService.BindService(new CommandHandlerImpl()) },
                Ports = { new ServerPort("localhost", port, ServerCredentials.Insecure) } // Needs to be changed later to a, have a better port and also to have a better ip/secure the connection
            };
            server.Start();

            Console.WriteLine("Server listening on port " + port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
