using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindTurbinesMod
{
    [Serializable]
    public struct WindTurbineConfig
    {
        public bool PositionInfluencesPower;
        public bool TurbineTakesDamage;
        public bool TurbineMakesNoise;
    }
}
