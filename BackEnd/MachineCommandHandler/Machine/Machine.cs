using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineCommandHandler
{
    class Machine
    {
        public bool isBeingUsed { get; set; }
        public bool needsToBeReset { get; set; }
        public string machineID { get; set; }
        public string connectedUserID { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="machineID"></param>
        public Machine(string machineID)
        {
            this.machineID = machineID;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>Is this still needed?</remarks>
        public bool CheckIfMachineIsBeingUsed()
        {
            return isBeingUsed;
        }
    }
}
