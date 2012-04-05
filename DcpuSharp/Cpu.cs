using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DcpuSharp
{
    public partial class Cpu
    {
        public Cpu(ushort[] initalMemory)
        {
            for (int i = 0; i < initalMemory.Length; i++)
            {
                Memory[i] = initalMemory[i];
            }
        }

        private readonly ushort[] Memory = new ushort[0x10000];
        private readonly ushort[] Registers = new ushort[8];
        private static readonly char[] RegisterNames = new char[] { 'A', 'B', 'C', 'X', 'Y', 'Z', 'I', 'J' };
        private ushort PC = 0;
        private ushort SP = 0xffff;
        private ushort Overflow = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if basic, false otherwise</returns>
        private bool GetInstr(out int op, out Value a, out Value b)
        {
            ushort instr = Memory[PC++];
            if ((instr & 0xf) == 0)
            {
                //non-basic
                op = (instr >> 4) & 0x3f;
                a = CreateValue((ushort)(instr >> 10));
                b = default(Value);
                return false;
            }
            else
            {
                //basic
                op = instr & 0xf;
                a = CreateValue((ushort)((instr >> 4) & 0x3f));
                b = CreateValue((ushort)(instr >> 10));
                return true;
            }
        }

        public void Tick()
        {
            int op;
            Value a, b;
            if (!GetInstr(out op, out a, out b))
            {
                //non-basic
                switch (op)
                {
                    case 0x1: //JSR
                        Memory[--SP] = PC;
                        PC = a.Get();
                        break;
                    default:
                        throw new UnsupportedInstructionException(0); //TODO: fix this
                }
            }
            else
            {
                //basic
                uint temp;
                switch (op)
                {
                    case 0x1: //SET
                        a.Set(b.Get());
                        break;
                    case 0x2: //ADD
                        temp = (uint)(a.Get() + b.Get());
                        a.Set((ushort)(temp & 0xffff));
                        Overflow = temp > 0xffff ? (ushort)1 : (ushort)0;
                        break;
                    case 0x3: //SUB
                        temp = (uint)(a.Get() - b.Get());
                        a.Set((ushort)(temp & 0xffff));
                        Overflow = temp > 0xffff ? (ushort)0xffff : (ushort)0;
                        break;
                    case 0x4: //MUL
                    case 0x5: //DIV
                    case 0x6: //MOD
                        throw new NotImplementedException();
                    case 0x7: //SHL
                        temp = (uint)(a.Get() << b.Get());
                        Overflow = (ushort)((temp >> 16) & 0xffff);
                        a.Set((ushort)temp);
                        break;
                    case 0x8: //SHR
                    case 0x9: //AND
                    case 0xa: //BOR
                    case 0xb: //XOR
                    case 0xc: //IFE
                        throw new NotImplementedException();
                    case 0xd: //IFN
                        if (!(a.Get() != b.Get()))
                        {
                            //Use GetInstr to get the side effect of advancing the PC,
                            //but don't let the SP change.
                            temp = SP;
                            GetInstr(out op, out a, out b);
                            SP = (ushort)temp;
                        }
                        break;
                    case 0xe: //IFG
                    case 0xf: //IFB
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
