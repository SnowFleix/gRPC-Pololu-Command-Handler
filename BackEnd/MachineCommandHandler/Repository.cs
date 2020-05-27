using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineCommandHandler
{
    // class created as a quick fix to share data between classes
    static class Repository
    {
        #region Test Code
#if DEBUG
        public static List<Servo> testServos { get; set; }
        public static Servo testServo { get; set; }
        public static TicMotor testMotor { get; set; }
#endif
        #endregion

        public static List<Machine> machines { get; set; }
        public const int move_motor_by_amount = 2000; // change later, was a quick fix to easily adjust the amound the motor moves by
        public const int move_servo_by_amount = 200;  // change later, was a quick fix to easily adjust the amound the servo moves by
        public static int current_machine_id = 0;

        /// <summary>
        /// Returns a machine with the corresponding machineID if it can't find the machine it returns null
        /// </summary>
        /// <param name="machineID">The unique id of the machine</param>
        /// <returns>Returns the machine with the corresponding id, if it can't find the machine it returns null</returns>
        public static Machine GetMachine(string machineID)
        {
            foreach (Machine m in machines)
                if (m.machineID == machineID)
                    return m;
            return null;
        }
    }
}
