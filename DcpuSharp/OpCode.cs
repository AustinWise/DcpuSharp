using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DcpuSharp
{
    public enum OpCode : ushort
    {
        //Basic
        Set = 0x1,
        Add = 0x2,
        Sub = 0x3,
        Mul = 0x4,
        Div = 0x5,
        Mod = 0x6,
        Shl = 0x7,
        Shr = 0x8,
        And = 0x9,
        Bor = 0xa,
        Xor = 0xb,
        IfE = 0xc,
        IfN = 0xd,
        IfG = 0xe,
        IfB = 0xf,

        //Non-basic
        JSR = 0x1 << 4,
    }
}
