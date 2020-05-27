using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineCommandHandler
{
    class SkeeBall : Machine
    {
        BrushedDCMotor cannonMotor;
        TicMotor xMotor;
        
        public SkeeBall(string machineID) : base(machineID)
        {
            
        }
    }
}
