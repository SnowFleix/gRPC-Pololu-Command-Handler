using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Grpc.Core;
using MachineCommandServiceProtoBuf;

namespace MachineCommandHandlerTester
{
    class Source
    {
        readonly MachineCommandService.MachineCommandServiceClient client;
        static Random random = new Random();

        static string sessionID = "0";
        static string machineID = "0";

        static int openThreads = 0;

        static List<MoveRequest> requests = new List<MoveRequest>();
        static List<MoveTestRequest> testRequests = new List<MoveTestRequest>();

        public Source(MachineCommandService.MachineCommandServiceClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Creates a new move request with the specified parameters
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="machineID"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static MoveRequest NewMoveRequest(string sessionID, string machineID, int direction)
        {
            return new MoveRequest
            {
                SessionID = sessionID,
                MachineID = machineID,
                Direction = direction
            };
        }

        /// <summary>
        /// Creates a new test servo request with a random taret between 4000 and 8000
        /// </summary>
        /// <returns></returns>
        public static MoveTestRequest NewTestMoveRequest(int tartgetServo)
        {
            return new MoveTestRequest
            {
                TargetPosition = random.Next(4000, 8000),
                TargetServo = tartgetServo
            };
        }

        /// <summary>
        /// Creates a new move request to test the motor movements
        /// </summary>
        /// <param name="tartgetServo"></param>
        /// <returns></returns>
        public static MoveTestRequest TestMotorMoveRequest()
        {
            return new MoveTestRequest
            {
                TargetPosition = random.Next(1, 1500)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public async Task MovingTest(List<MoveRequest> requests)
        {
            using (var MoveClawStreamTest = client.MoveClaw())
            {
                // 
                var responseTask = Task.Run(async () =>
                {
                    // 
                    while (await MoveClawStreamTest.ResponseStream.MoveNext())
                    {
                        // 
                        var note = MoveClawStreamTest.ResponseStream.Current;
                    }
                });
                // 
                foreach (MoveRequest request in requests)
                {
                    await MoveClawStreamTest.RequestStream.WriteAsync(request);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public async Task SkiballMovingTest(List<MoveRequest> requests)
        {
            using (var MoveCannonStreamTest = client.MoveCannon())
            {
                // 
                var responseTask = Task.Run(async () =>
                {
                    // 
                    while (await MoveCannonStreamTest.ResponseStream.MoveNext())
                    {
                        // 
                        var note = MoveCannonStreamTest.ResponseStream.Current;
                    }
                });
                // 
                foreach (MoveRequest request in requests)
                {
                    await MoveCannonStreamTest.RequestStream.WriteAsync(request);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public async Task TestStreamServoMovements(List<MoveTestRequest> requests)
        {
            try
            {
                using (var ServoCommandStreamTest = client.StreamMovementsTest())
                {
                    // 
                    var responseTask = Task.Run(async () =>
                    {
                        // 
                        while (await ServoCommandStreamTest.ResponseStream.MoveNext())
                        {
                            // 
                            var note = ServoCommandStreamTest.ResponseStream.Current;
                            Console.WriteLine(note.Fulfilled);
                        }
                    });
                    // 
                    foreach (MoveTestRequest request in requests)
                    {
                        await ServoCommandStreamTest.RequestStream.WriteAsync(request);
                        Console.WriteLine(request.ToString());
                    }
                    await ServoCommandStreamTest.RequestStream.CompleteAsync();
                    await responseTask;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requests"></param>
        /// <returns></returns>
        public async Task TestStreamMotorMovements(List<MoveTestRequest> requests)
        {
            try
            {
                using (var MotorCommandStreamTest = client.StreamMotorMovementsTest())
                {
                    // 
                    var responseTask = Task.Run(async () =>
                    {
                        // 
                        while (await MotorCommandStreamTest.ResponseStream.MoveNext())
                        {
                            // 
                            var note = MotorCommandStreamTest.ResponseStream.Current;
                            //Console.WriteLine(note.Fulfilled);
                        }
                    });
                    // 
                    foreach (MoveTestRequest request in requests)
                    {
                        await MotorCommandStreamTest.RequestStream.WriteAsync(request);
                        //Console.WriteLine(request.ToString());
                    }
                    await MotorCommandStreamTest.RequestStream.CompleteAsync();
                    await responseTask;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// Tests each of the RPC function calls
        /// </summary>
        /// <param name="client">The client to connect with</param>
        /// <remarks>
        /// Stops the duplication of code
        /// TODO: does not include the two bidirectional streaming RPCs
        /// </remarks>
        public static void TestRPCs(Source client)
        {
            //var MoveServoReply                          = client.client.MoveServo(new MoveServoTestRequest { TargetPosition = 4000 });
            //Thread.Sleep(1000);
            //MoveServoReply                              = client.client.MoveServo(new MoveServoTestRequest { TargetPosition = 8000 });
            var DropClawReply = client.client.DropClaw(new DropClawRequest { SessionID = sessionID, MachineID = machineID });
            var AvaliableMachinesReply = client.client.GetAvailableMachines(new GetAvailableMachinesRequest { SessionID = sessionID });
            var CheckingIfMachineIsAvaliableReply = client.client.CheckIfMachineIsAvailable(new IsMachineAvailableRequest { MachineID = machineID });
            var FireCannonReply = client.client.FireSkiBall(new FireSkiBallRequest { SessionID = sessionID, MachineID = machineID });
            var ConnectToMachineReply = client.client.ConnectToMachine(new ConnectToMachineRequest { SessionID = sessionID, MachineID = machineID });
            var DisconnectFromMachineReply = client.client.DisconnectFromMachine(new DisconnectFromMachineRequest { MachineID = machineID, SessionID = sessionID });
        }

        /// <summary>
        /// 
        /// </summary>
        public static void TestMotorAsync()
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure); // currently unsecure, will be changed later

            // creates a new connection/channel as if it were a new user
            var client = new Source(new MachineCommandService.MachineCommandServiceClient(channel));

            List<MoveTestRequest> testRequestsConcur = new List<MoveTestRequest>();

            for (int i = 0; i < 50; i++)
                testRequestsConcur.Add(TestMotorMoveRequest());

            client.TestStreamMotorMovements(testRequestsConcur).Wait();

            channel.ShutdownAsync().Wait();

            Console.WriteLine("Motor Test Thread Shutdown");
            openThreads--;
        }

        /// <summary>
        /// function used when testing concurrent connections to the server
        /// </summary>
        public static void ConcurrentTest()
        {
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure); // currently unsecure, will be changed later

            // creates a new connection/channel as if it were a new user
            var client = new Source(new MachineCommandService.MachineCommandServiceClient(channel));

            // Bombardment of requests to test how the system copes with concurrency
            for (int i = 0; i < 100; i++)
                TestRPCs(client);

            channel.ShutdownAsync().Wait();

            Console.WriteLine("Thread shutdown");
            openThreads--;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ConcurrentServoTest(int servoPos)
        {
            Console.WriteLine("New Thread - Servo Pos: " + servoPos);

            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure); // currently unsecure, will be changed later

            // creates a new connection/channel as if it were a new user
            var client = new Source(new MachineCommandService.MachineCommandServiceClient(channel));

            List<MoveTestRequest> testRequestsConcur = new List<MoveTestRequest>();

            for (int i = 0; i < 10; i++)
                testRequestsConcur.Add(NewTestMoveRequest(servoPos));

            client.TestStreamServoMovements(testRequestsConcur).Wait();

            channel.ShutdownAsync().Wait();

            Console.WriteLine("Thread shutdown");
            openThreads--;
        }

        public static void Main(string[] args)
        {
            // creates the connection to the server
            Channel channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure); // currently unsecure, will be changed later

            var client = new Source(new MachineCommandService.MachineCommandServiceClient(channel));

            // Creates the simulated data
            for (int i = 0; i < 200; i++)
                requests.Add(NewMoveRequest(sessionID, machineID, random.Next(1, 5)));
            for (int i = 0; i < 200; i++)
                testRequests.Add(NewTestMoveRequest(-1));

            /* Code used to test my servo and motor controllers that won't work unless they are connected
            // Tests if a servo can have movements streamed to it
            client.TestStreamServoMovements(testRequests).Wait();

            Thread motorThread = new Thread(new ThreadStart(TestMotorAsync));
            motorThread.Start();
            openThreads++;

            // Tests if a maestro with 4 servos connected to it can have all its servos connected to and moved concurently
            for (int i = 0; i < 4; i++)
            {
                int currentServo = i; // for some reason even though I thought it was pass by value i would change once the new thread started
                Thread thread = new Thread(new ThreadStart(() => ConcurrentServoTest(currentServo))) ;
                thread.Start();
                openThreads++;
            }

            while (openThreads != 0) { }*/
            
            // Single test to diagnose problems with each result
            TestRPCs(client);

            // Bombardment of requests to test how the system copes
            for (int i = 0; i < 100; i++)
                TestRPCs(client);

            // concurrency test to see how the system handles mutiple concurrenct connections and requests
            for (int i = 0; i < 100; i++)
            {
                Thread thread = new Thread(new ThreadStart(ConcurrentTest));
                thread.Start();
                openThreads++;
            }

            //client.movingtest(requests).wait();
            //client.skiballmovingtest(requests).wait();
            channel.ShutdownAsync().Wait();
            while (openThreads != 0) { }
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}