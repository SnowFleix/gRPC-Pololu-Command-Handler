using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pololu.Usc;
using Pololu.UsbWrapper;

using static MachineCommandHandler.Repository;

namespace MachineCommandHandler
{
    class Maestro
    {
        // represents a maestro that's connected
        private Usc usc = null;
        private ServoStatus[] servos;
        private readonly string serialNumber;

#region TestCode
#if DEBUG
        public Maestro(string serialNo)
        {
            this.serialNumber = serialNo;
        }
#endif
#endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usc"></param>
        /// <param name="serialNo"></param>
        public Maestro(Usc usc, string serialNo)
        {
            this.serialNumber = serialNo;
            this.usc = usc;
            usc.getVariables(out servos);
        }

        /// <summary>
        /// Connects to the device if it is found in the device list.
        /// </summary>
        public void TryToReconnect()
        {
            foreach (DeviceListItem d in Usc.getConnectedDevices())
            {
                if (d.serialNumber == serialNumber)
                {
                    usc = new Usc(d);
                    //Log("Connected to #" + SerialNumberTextBox.Text + ".");
                    return;
                }
            }
        }

        /// <summary>
        /// Attempts to disconnect
        /// </summary>
        public void TryToDisconnect()
        {
            if (usc == null)
            {
                //Log("Connecting stopped.");
                return;
            }

            try
            {
                //Log("Disconnecting...");
                usc.Dispose();  // Disconnect
            }
            catch (Exception e)
            {
                //Log(e);
                //Log("Failed to disconnect cleanly.");
            }
            finally
            {
                // do this no matter what
                usc = null;
                //Log("Disconnected from #" + SerialNumberTextBox.Text + ".");
            }
        }

        /// <summary>
        /// Gets the position of a specific servo connected to the maestro
        /// </summary>
        /// <param name="servoNo">The position of the servo in the maestro</param>
        /// <returns>Returns the position</returns>
        public int GetServoPosition(int servoNo)
        {
            // update the servo status first
            usc.getVariables(out servos);
            return (int)servos[servoNo].position;
        }

        /// <summary>
        /// Moves the specified servo to the target position
        /// </summary>
        /// <param name="servoNo">Position where the servo is connected to the maestro</param>
        /// <param name="target">How much to rotate the servo</param>
        public void MoveServo(int servoNo, int target)
        {
            usc.setTarget((byte)servoNo, (ushort)target);
        }

        /// <summary>
        /// Gets the positions where a servo is connected to the maestro
        /// </summary>
        /// <returns>Read only list of integers where servos are connected</returns>
        public IReadOnlyList<int> ServosConnected()
        {
            List<int> retLst = new List<int>();
            usc.getVariables(out servos);
            for (int i = 0; i < servos.Length; i++)
                if (servos[i].position != 0)
                    retLst.Add(i);
            return retLst;
        }
    }
}
