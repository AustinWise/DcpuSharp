using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DcpuSharp
{
    class CodeWriter
    {
        private ushort[] mMemory = new ushort[0x10000];
        public void EmitBasic(OpCode op, Value a, Value b)
        {
        }
    }
}
