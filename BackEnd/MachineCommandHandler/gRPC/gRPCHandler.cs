using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using Grpc.Core; // 
using MachineCommandServiceProtoBuf; // class made by the protobuff

using static MachineCommandHandler.Repository; // used for sharing the data between classes

namespace MachineCommandHandler
{
    class CommandHandlerImpl : MachineCommandService.MachineCommandServiceBase
    {
        #region Util requests
        /// <summary>
        /// Checks each machine for its availability and returns the list to the client
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<GetAvailableMachinesResponse> GetAvailableMachines(GetAvailableMachinesRequest request, ServerCallContext context)
        {
            #region TestCode
            Console.WriteLine("Get available machines Request: " + request.ToString());
            #endregion
            GetAvailableMachinesResponse response = new GetAvailableMachinesResponse();
            foreach (Machine m in machines)
                if (!m.isBeingUsed)
                {
                    if (m is ClawMachine c)
                        response.AvailableClawMachines.Add(new MachineCommandServiceProtoBuf.ClawMachine { MachineID = c.machineID });
                    if (m is SkeeBall s)
                        response.AvailableSkeeBallMachines.Add(new MachineCommandServiceProtoBuf.SkeeBallMachine { MachineID = s.machineID });
                    if (m is MilkBottle milk)
                        response.AvailableMilkBottleMachines.Add(new MachineCommandServiceProtoBuf.MilkBottleMachine { MachineID = milk.machineID });
                }
            return Task.FromResult(response);
        }

        /// <summary>
        /// Checks if a specific machine is available 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<IsMachineAvailableResponse> CheckIfMachineIsAvailable(IsMachineAvailableRequest request, ServerCallContext context)
        {
            #region TestCode
            Console.WriteLine("Check if machine is available Request: " + request.ToString());
            #endregion
            foreach (Machine m in machines)
                if (m.machineID == request.MachineID)
                    return Task.FromResult(new IsMachineAvailableResponse { IsAvailable = m.isBeingUsed });
            return Task.FromResult(new IsMachineAvailableResponse { IsAvailable = false });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ConnectToMachineResponse> ConnectToMachine(ConnectToMachineRequest request, ServerCallContext context)
        {
            #region TestCode
            Console.WriteLine("Connect to machine Request: " + request.ToString());
            #endregion
            foreach (Machine m in machines)
                if (m.machineID == request.MachineID)
                {
                    m.isBeingUsed = true;
                    m.connectedUserID = request.SessionID;
                    return Task.FromResult(new ConnectToMachineResponse { Fulfilled = true });
                }
            return Task.FromResult(new ConnectToMachineResponse { Fulfilled = false });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<DisconnectFromMachineResponse> DisconnectFromMachine(DisconnectFromMachineRequest request, ServerCallContext context)
        {
            #region TestCode
            Console.WriteLine("Disconnect from machine Request: " + request.ToString());
            #endregion
            foreach (Machine m in machines)
                if (m.machineID == request.MachineID)
                {
                    m.isBeingUsed = false;
                    return Task.FromResult(new DisconnectFromMachineResponse { Fulfilled = true });
                }
            return Task.FromResult(new DisconnectFromMachineResponse { Fulfilled = false });
        }
        #endregion

        #region Claw game control
        /// <summary>
        /// Handles the moving of the claw
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task MoveClaw(IAsyncStreamReader<MoveRequest> requestStream, IServerStreamWriter<MoveResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var curCommand = requestStream.Current;
                ClawMachine curMachine = (ClawMachine)GetMachine(curCommand.MachineID); // explicit cast, the machine id should return a claw machine if it is attempting to move the claw
                // using a try catch as if the controller throws an error it is impreritive the stream doesn't crash
                try
                {
                    switch (curCommand.Direction)
                    {
                        case 1:
                            curMachine.MoveLeft();
                            break;
                        case 2:
                            curMachine.MoveRight();
                            break;
                        case 3:
                            curMachine.MoveForward();
                            break;
                        case 4:
                            curMachine.MoveBack();
                            break;
                        default:
                            await responseStream.WriteAsync(new MoveResponse { Fulfilled = false });
                            continue;
                    }
                    await responseStream.WriteAsync(new MoveResponse { Fulfilled = true });
                }
                catch (Exception e)
                {
                    await responseStream.WriteAsync(new MoveResponse { Fulfilled = false });
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<DropClawResponse> DropClaw(DropClawRequest request, ServerCallContext context)
        {
            #region TestCode
            Console.WriteLine("Drop Claw Request: " + request.ToString());
            #endregion
            return Task.FromResult(new DropClawResponse { Fulfilled = false });
        }
        #endregion

        #region Skiball game control
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task MoveCannon(IAsyncStreamReader<MoveRequest> requestStream, IServerStreamWriter<MoveResponse> responseStream, ServerCallContext context)
        {
            await responseStream.WriteAsync(new MoveResponse { });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Task<FireSkiBallResponse> FireSkiBall(FireSkiBallRequest request, ServerCallContext context)
        {
            #region TestCode
            Console.WriteLine("Fire SkiBall Request: " + request.ToString());
            #endregion
            return Task.FromResult(new FireSkiBallResponse { });
        }
        #endregion

        #region Test servo movement
#if DEBUG
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<MoveTestResponse> MoveServo(MoveTestRequest request, ServerCallContext context)
        {
            #region TestCode
            Console.WriteLine("Move test servo Request: " + request.ToString());
            #endregion
            testServo.MoveServo(request.TargetPosition);
            return Task.FromResult(new MoveTestResponse { Fulfilled = true });
        }

        /// <summary>
        /// Tests how the servos work with a stream of asynchronous requests to move the servo
        /// This would simulate a typical usecase of the user attempting to move a servo, e.g. aiming the 
        /// skeeball cannon
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamMovementsTest(IAsyncStreamReader<MoveTestRequest> requestStream, IServerStreamWriter<MoveTestResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var curCommand = requestStream.Current;
                #region TestCode
                Console.WriteLine("Move test servo move Request: " + curCommand.ToString());
                int originalPosition = testServo.GetServoPosition();
                #endregion
                if (curCommand.TargetServo == -1)
                    testServo.MoveServo(curCommand.TargetPosition);
                else
                    testServos[curCommand.TargetServo].MoveServo(curCommand.TargetPosition);
                // The original problem this fixed is that the getposition will always return the target position even if it is in the process of moving
                // this means I cannot set the target position then wait for it to reach that position then move onto the next, it turns out that it takes
                // roughly 400ms to go from 4000 to 8000 on the servo, so I just use the target position 
                Thread.Sleep(Math.Abs(curCommand.TargetPosition - originalPosition) / 10);
                await responseStream.WriteAsync(new MoveTestResponse { Fulfilled = true });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamMotorMovementsTest(IAsyncStreamReader<MoveTestRequest> requestStream, IServerStreamWriter<MoveTestResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var curCommand = requestStream.Current;
                #region TestCode
                Console.WriteLine("Move test motor move Request: " + curCommand.ToString());
                #endregion
                testMotor.SetPosition(curCommand.TargetPosition);
                await responseStream.WriteAsync(new MoveTestResponse { Fulfilled = true });
            }
        }
#endif
        #endregion
    }
}
