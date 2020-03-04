using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Gamma
{
    static class TryConvert
    {
        public static bool TryToByte(string value, int fromBase, out byte result)
        {
            result = default;
            try {
                result = Convert.ToByte(value, fromBase);
            }
            catch {
                return false;
            }
            return true;
        }

        public static bool TryToByte(string value, out byte result)
        {
            result = default;
            try {
                Convert.ToByte(value);
            }
            catch {
                return false;
            }
            return true;
        }
    }
}
