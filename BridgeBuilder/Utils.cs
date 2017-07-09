using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeBuilder
{
    static class Utils
    {
        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
