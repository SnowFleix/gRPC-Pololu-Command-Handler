using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static MachineCommandHandler.Repository;

namespace MachineCommandHandler
{
    class SkeeBall : Machine
    {
        BrushedDCMotor cannonMotor;
        Servo aimServo, fireServo;
        TicMotor xMotor;
        int currentPoints;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="machineID"></param>
        /// <param name="cannonMotor"></param>
        /// <param name="aimServo"></param>
        /// <param name="fireServo"></param>
        /// <param name="xMotor"></param>
        public SkeeBall(string machineID, BrushedDCMotor cannonMotor, Servo aimServo, Servo fireServo, TicMotor xMotor) : base(machineID)
        {
            this.cannonMotor = cannonMotor;
            this.aimServo = aimServo;
            this.xMotor = xMotor;
            this.fireServo = fireServo;
            this.cannonMotor.SetSpeed((int)BrushedDCMotor.CONSTANTS.MAX_SPEED);
        }

        /// <summary>
        /// Swivels the cannon left
        /// </summary>
        public void AimCannonLeft()
        {
            if (aimServo.GetServoPosition() == (int)Servo.CONSTANTS.MIN_POSITION)
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
