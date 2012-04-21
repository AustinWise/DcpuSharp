using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DcpuSharp
{
    public enum Register : byte
    {
        A = 0,
        B,
        C,
        X,
        Y,
        Z,
        I,
        J,

        SP = 0x1b,
        PC = 0x1c,
        O = 0x1d,
    }
}
