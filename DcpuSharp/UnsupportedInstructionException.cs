using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DcpuSharp
{
    public class UnsupportedInstructionException : NotSupportedException
    {
        public UnsupportedInstructionException(ushort instr)
            : base("This instruction in is not supported: 0x" + instr.ToString("x"))
        {
        }
    }
}
