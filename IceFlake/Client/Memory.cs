using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using IceFlake.Client.Patchables;
using IceFlake.Runtime;

namespace IceFlake.Client
{
    public static class Memory
    {

        public static string ConvertToHexString(uint value)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder("0x");
            builder.Append(Convert.ToString(value, 16).ToUpper());
            return builder.ToString();
        }


    }
}