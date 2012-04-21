using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DcpuSharp
{
    class CodeWriter
    {
        const int MemorySize = 0x10000;

        private int mLength = 0;
        private ushort[] mMemory = new ushort[MemorySize];
        private List<int> mLabels = new List<int>();
        private List<Fixup> mFixups = new List<Fixup>();

        public void EmitBasic(OpCode op, Value a, Value b)
        {
            byte byteOp = (byte)op;
            if ((byteOp & 0xf) != byteOp)
                throw new ArgumentOutOfRangeException("op", "op needs to be a basic op.");
            int instr = (byte)op;
            instr |= a.Encode() << 4;
            instr |= b.Encode() << 10;

            mMemory[mLength++] = (ushort)instr;
            var aNext = a.GetNextWord();
            var bNext = b.GetNextWord();
            if (aNext.HasValue)
                mMemory[mLength++] = aNext.Value;
            if (bNext.HasValue)
                mMemory[mLength++] = bNext.Value;
        }

        public ushort[] ToArray()
        {
            ushort[] ret = new ushort[MemorySize];
            Array.Copy(mMemory, ret, MemorySize);
            return ret;
        }
    }
}
