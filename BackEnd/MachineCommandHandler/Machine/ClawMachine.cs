using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pololu.Usc;

using static MachineCommandHandler.Repository;

namespace MachineCommandHandler
{
    class ClawMachine : Machine
    {
        // grabs the ids for all the servos and motors
        private Servo clawServo; // the maestro controller that the servo is connected to
        private TicMotor xMotor, yMotor, winchMotor; // the motors attatched to the machine
        private string webcamName; // might need to be an int or called webcamID

        #region TestCode
        #if DEBUG
        /// <summary>
        /// Test code, used for testing the gRPC connection, allows the claw machine to be created without any parameters
        /// </summary>
        /// <param name="machineID">Unique machine id</param>
        public ClawMachine(string machineID) : base(machineID)
        {
            this.clawServo = new Servo(new Maestro(""), 0);
            this.xMotor = new TicMotor();
            this.yMotor = new TicMotor();
            this.winchMotor = new TicMotor();
            this.webcamName = "";
        }
        #endif
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="machineID">The unique id for this machine</param>
        /// <param name="clawServo">The servo connected to the claw</param>
        /// <param name="motorX">TicMotor for handling the x axis of the claw</param>
        /// <param name="motorY">TicMotor for handling the y axis of the claw</param>
        /// <param name="winchMotor">TicMotor for handling the winch</param>
        /// <param name="webcamName">The unique ID that points to the webcam</param>
        /// <remarks>x and y axis are from the top down view</remarks>
        public ClawMachine(string machineID, Servo clawServo, TicMotor motorX, TicMotor motorY, TicMotor winchMotor, string webcamName) : base(machineID)
        {
            this.clawServo = clawServo;
            this.xMotor = motorX;
            this.yMotor = motorY;
            this.winchMotor = winchMotor;
            this.webcamName = webcamName;
        }

        /// <summary>
        /// Moves the claw to the right in reference to the front camera
        /// </summary>
        public void MoveRight()
        {
            xMotor.SetPosition(xMotor.GetPosition() + move_motor_by_amount);
        }

        /// <summary>
        /// Moves the claw to the left in reference to the front camera
        /// </summary>
        public void MoveLeft()
        {
            xMotor.SetPosition(xMotor.GetPosition() - move_motor_by_amount);
            
        }

        /// <summary>
        /// Moves the claw towards the front camera
        /// </summary>
        public void MoveForward()
        {
            yMotor.SetPosition(yMotor.GetPosition() + move_motor_by_amount);
        }

        /// <summary>
        /// Moves the claw backwards in reference to the front camera
        /// </summary>
        public void MoveBack()
        {
            yMotor.SetPosition(yMotor.GetPosition() - move_motor_by_amount);
        }

        /// <summary>
        /// Drops the claw and attempts to grab the item as well as resets the claw
        /// </summary>
        /// <remarks> Add a way to handle errors/if something happens </remarks>
        public bool DropAndGrabClaw()
        {
            clawServo.MoveServo((int)Servo.CONSTANTS.MAX_POSITION);         // opens the claw
            winchMotor.SetPosition(100000);                                 // lowers the claw TODO: check if this is the correct position
            clawServo.MoveServo((int)Servo.CONSTANTS.MIN_POSITION);         // closes the claw
            winchMotor.SetPosition((int)TicMotor.CONSTANTS.MIN_POSITION);   // winches up the claw TODO: check if this is the correct position
            xMotor.SetPosition((int)TicMotor.CONSTANTS.MIN_POSITION);       // resets the claws x axis
            yMotor.SetPosition((int)TicMotor.CONSTANTS.MIN_POSITION);       // resets the claws y axis

            // TODO: make sure it's able to find 0 for all of the motors
            while (xMotor.GetPosition() != (int)TicMotor.CONSTANTS.MIN_POSITION // 
                && yMotor.GetPosition() != (int)TicMotor.CONSTANTS.MIN_POSITION // 
                && winchMotor.GetPosition() != (int)TicMotor.CONSTANTS.MIN_POSITION); // wait until the claw is back at the start point

            return true;
        }

        /// <summary>
        /// Gets the webcam name 
        /// </summary>
        /// <returns>The unique ID assigned to the webcam/live video feed</returns>
        public string GetWebcamName()
        {
            return webcamName;
        }
    }
}
