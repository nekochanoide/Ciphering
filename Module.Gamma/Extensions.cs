using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Gamma
{
    static class Extensions
    {
        public static string ToStr(this IEnumerable<byte> bytes)
        {
            var result = new StringBuilder();
            foreach(var e in bytes)
                result.Append(e.ToString("x2"));
            return result.ToString();
        }

        public static byte[] ReadBytes(this string value)
        {
            if (value.Length % 2 != 0)
                return null;
            var bytes = new List<byte>();
            for (int i = 0; i < value.Length; i += 2)
            {
                if (TryConvert.TryToByte(value.Substring(i, 2), 16, out byte @byte))
                    bytes.Add(@byte);
                else
                    return null;
            }
            return bytes.ToArray();
        }
    }
}
