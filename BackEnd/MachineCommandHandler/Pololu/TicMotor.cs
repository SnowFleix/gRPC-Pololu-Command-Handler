using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineCommandHandler
{
    class TicMotor
    {
        #region Constants
        public enum CONSTANTS
        {
            MAX_POSITION = 100000,      // TODO: Check if the motor can get to this spot and where it is 
            MIN_POSITION = 0,           // TODO: Check if the motor can go to >0
            MAX_VELOCITY = 9500000,     // TODO: Check if the motor can go this fast
            MIN_VELOCITY = 0,           // Min is currently 0 but may change
            MAX_ACCELERATION = 100000,
            MIN_ACCELERATION = 0
        }
        #endregion

        tic ticController = new tic(); // 
        string serial = "";
        bool isConnected = false;

        #region TestCode
#if DEBUG
        /// <summary>
        /// 
        /// </summary>
        public TicMotor() { }
#endif
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="serialNo">The serial number of the controller</param>
        /// <remarks>TODO: make sure all errors are handled so the server can run smoothly</remarks>
        public TicMotor(string serialNo)
        {
            this.serial = serialNo;
            try
            {
                TryReconnect(); // open the motor so we can handle errors and set the max velocity
                ticController.set_max_speed((int)CONSTANTS.MAX_VELOCITY);
                ticController.set_starting_speed(5000000);
                ticController.set_max_accel(3000000);
                ticController.set_max_decel(3000000);
            }
            catch (Exception e)
            {
                Console.Write("[TicMotorError] : " + e.StackTrace);
            }
        }

        /// <summary>
        /// Move the motor to a specific position
        /// </summary>
        /// <param name="pos">Position where the motor needs to go to</param> <!-- need to explain more about the value and if there is a max? -->
        public bool SetPosition(int pos)
        {
            if (ticController.get_variables())
                if (ticController.status_vars.energized)
                {
                    // Check if the pos is less than 0 as the motors home value should always be 0
                    if (pos <= 0)
                        pos = 0;
                    ticController.set_target_position(pos);
                    while (ticController.get_variables()) { if (ticController.vars.current_position == pos) break; } // 
                }
                else return false;
            return true;
        }

        /// <summary>
        /// Gets the current position of the stepper motor and returns -1 if unobtainable
        /// </summary>
        /// <returns>The current position of the stepper motor, if the function returns -1 it means the position could not be found</returns>
        public int GetPosition()
        {
            if (ticController.get_variables())
                return ticController.vars.current_position;
            else
                return -1; // the position should not be -1 therefore return it if the position cannot be obtained
        }

        /// <summary>
        /// Set the speed of the stepper motor
        /// </summary>
        /// <param name="velocity">The velocity of the stepper motor</param> <!-- the max value for the peram is 100000.000 pulses/s -->
        public void SetVelocity(int velocity)
        {
            ticController.set_target_velocity(velocity);
        }

        /// <summary>
        /// Stops the motor
        /// </summary>
        public void Halt()
        {
            ticController.halt_and_hold();
        }

        /// <summary>
        /// Attempts to connect to the tic motor controller
        /// </summary>
        public void TryReconnect()
        {
            // TODO: need to check if this just gets the first one connected
            // perhaps use the serial of the controller?
            // need to find all the serials of devices connected through usb
            isConnected = ticController.open(tic.PRODUCT_ID.T500, serial);
        }

        /// <summary>
        /// Attempts to power down and disconnect from the stepper motor
        /// </summary>
        public void TryDisconnect()
        {
            ticController.deenergize();
            ticController.close();
        }
    }
}
