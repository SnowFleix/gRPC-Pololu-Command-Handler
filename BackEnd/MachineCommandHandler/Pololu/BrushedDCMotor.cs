using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pololu.UsbWrapper;
using Pololu.SimpleMotorController;

namespace MachineCommandHandler
{
    class BrushedDCMotor
    {
        #region Constants
        public enum CONSTANTS
        {
            MAX_SPEED = 8000,
            MIN_SPEED = 4000,
        }
        #endregion

        private Smc motor = null;
        private string serialNumber;

        /// <summary>
        /// Default constructor
        /// </summary>
        public BrushedDCMotor(string serialNo)
        {
            serialNumber = serialNo;
            TryToReconnect();                               // Try to connect to the device to check if it exists
            motor.resume();                                 // 
            motor.setSpeed((short)CONSTANTS.MAX_SPEED);     // Set the speed and direction using the constants
        }

        /// <summary>
        /// Change the speed the motor is running at
        /// </summary>
        /// <param name="speed"></param>
        public void SetSpeed(int speed)
        {
            motor.resume();
            motor.setSpeed((short)speed);
        }

        /// <summary>
        /// Stops the motor moving
        /// </summary>
        public void StopMotor()
        {
            motor.stop();   // this however activates the USB kill switch, perhaps change this to SetSpeed(0)?
        }

        /// <summary>
        /// Connects to the device if it is found in the device list.
        /// </summary>
        public void TryToReconnect()
        {
            foreach (DeviceListItem d in Smc.getConnectedDevices())
            {
                if (d.serialNumber == serialNumber)
                {
                    motor = new Smc(d);
                    return;
                }
            }
            throw new Exception("Cannot connect to the SMC Make sure it is plugged in to USB " +
                "and check your Device Manager (Windows)");
        }

        /// <summary>
        /// Attempts to disconnect
        /// </summary>
        public void TryToDisconnect()
        {
            if (motor == null)
            {
                //Log("Connecting stopped.");
                return;
            }

            try
            {
                //Log("Disconnecting...");
                motor.Dispose();  // Disconnect
            }
            catch (Exception e)
            {
                //Log(e);
                //Log("Failed to disconnect cleanly.");
            }
            finally
            {
                // do this no matter what
                motor = null;
                //Log("Disconnected from #" + SerialNumberTextBox.Text + ".");
            }
        }
    }
}
