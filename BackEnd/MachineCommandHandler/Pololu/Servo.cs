using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineCommandHandler
{
    class Servo
    {
        #region Constants
        public enum CONSTANTS
        {
            MAX_POSITION = 8000,
            MIN_POSITION = 4000,
        }
        #endregion

        Maestro connectedMaestro;
        int servoNo;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connectedMaestro">The maestro that the servo is connected to</param>
        /// <param name="serverNo">The position where the servo is</param>
        public Servo(Maestro connectedMaestro, int serverNo)
        {
            this.connectedMaestro = connectedMaestro;
            this.servoNo = serverNo;
        }

        /// <summary>
        /// Gets the position of a specific servo connected to the maestro
        /// </summary>
        /// <param name="servoNo">The position of the servo in the maestro</param>
        /// <returns>Returns the position</returns>
        public int GetServoPosition()
        {
            return (int)connectedMaestro.GetServoPosition(servoNo);
        }

        /// <summary>
        /// Moves the specified servo to the target position
        /// </summary>
        /// <param name="servoNo">Position where the servo is connected to the maestro</param>
        /// <param name="target">How much to rotate the servo</param>
        public void MoveServo(int target)
        {
            connectedMaestro.MoveServo(servoNo, target);
        }
    }
}
