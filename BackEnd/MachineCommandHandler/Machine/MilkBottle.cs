using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MachineCommandHandler.Repository;

namespace MachineCommandHandler
{
    class MilkBottle : Machine
    {
        BrushedDCMotor fireMotor;
        TicMotor xMotor;
        Servo aimServo, fireServo;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="machineID">The unique ID assigned to this machine</param>
        /// <param name="cannonMotor">The motor that controls the power of the cannon</param>
        /// <param name="xMotor">The motor that controlls the xy axis of the cannon</param>
        /// <param name="aimingServo">The servo used for aiming the cannon</param>
        /// <param name="firingServo">The servo used for pushing the ammo into the motor to be fired</param>
        public MilkBottle(string machineID, BrushedDCMotor cannonMotor, TicMotor xMotor, Servo aimingServo, Servo firingServo) : base(machineID)
        {
            this.fireMotor = cannonMotor;
            this.xMotor = xMotor;
            this.aimServo = aimingServo;
            this.fireServo = firingServo;
            this.fireMotor.SetSpeed((int)BrushedDCMotor.CONSTANTS.MAX_SPEED);
        }

        /// <summary>
        /// Swivels the cannon left
        /// </summary>
        public void AimCannonLeft()
        {
            if(aimServo.GetServoPosition() == (int)Servo.CONSTANTS.MIN_POSITION)
                aimServo.MoveServo(aimServo.GetServoPosition() - move_motor_by_amount);
        }

        /// <summary>
        /// Swivels the cannon right
        /// </summary>
        public void AimCannonRight()
        {
            if (aimServo.GetServoPosition() == (int)Servo.CONSTANTS.MAX_POSITION)
                aimServo.MoveServo(aimServo.GetServoPosition() + move_motor_by_amount);
        }
        
        /// <summary>
        /// Moves the cannon right along the x axis
        /// </summary>
        public void MoveCanonRight()
        {
            if (xMotor.GetPosition() == (int)TicMotor.CONSTANTS.MAX_POSITION)
                xMotor.SetPosition(xMotor.GetPosition() + move_motor_by_amount);
        }

        /// <summary>
        /// Moves the cannon left along the x axis
        /// </summary>
        public void MoveCannonLeft()
        {
            if (xMotor.GetPosition() == (int)TicMotor.CONSTANTS.MIN_POSITION)
                xMotor.SetPosition(xMotor.GetPosition() - move_motor_by_amount);
        }

        /// <summary>
        /// Fires the cannon and then resets the game
        /// </summary>
        public void FireCannon()
        {
            fireServo.MoveServo((int)Servo.CONSTANTS.MAX_POSITION);                         // pushes the ball into the motor to be fired
            while (fireServo.GetServoPosition() != (int)Servo.CONSTANTS.MAX_POSITION) ;     // wait until it's properly positioned 
            fireServo.MoveServo((int)Servo.CONSTANTS.MIN_POSITION);                         // resets the servo so it can fire the next 
            while (fireServo.GetServoPosition() != (int)Servo.CONSTANTS.MIN_POSITION) ;     // wait until it's properly positioned 
            if (!IsLoaded())
                needsToBeReset = true;
            if (PlayerWon())
                needsToBeReset = true;
            xMotor.SetPosition((int)TicMotor.CONSTANTS.MIN_POSITION);
        }

        /// <summary>
        /// Checks if the cannon has enough ammo
        /// </summary>
        /// <returns> Returns a bool </returns> <!-- Explain more -->
        /// <remarks> TODO: needs to be finished, need to test hardware</remarks>
        private bool IsLoaded()
        {
            return true;
        }

        /// <summary>
        /// Checks if the user has won
        /// </summary>
        /// <returns></returns>
        /// <remarks> TODO: needs to be finished, need to test hardware</remarks>
        private bool PlayerWon()
        {
            return false;
        }
    }
}
