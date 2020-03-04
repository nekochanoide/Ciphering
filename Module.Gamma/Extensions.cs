using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module.Gamma
{
    static class Extensions
    {
        public static string ToStr(this byte[] bytes)
        {
            var result = new StringBuilder();
            foreach(var e in bytes)
                result.Append(e.ToString("x2"));
            return result.ToString();
        }
    }
}
