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
        private static readonly string[] RegisterNames = new string[] { "A", "B", "C", "X", "Y", "Z", "I", "J" };
        private ushort PC = 0;
        private ushort SP = 0;
        private ushort Overflow = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="op"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>true if basic, false otherwise</returns>
        private bool GetInstr(out int op, out InternalValue a, out InternalValue b)
        {
            ushort instr = Memory[PC++];
            if ((instr & 0xf) == 0)
            {
                //non-basic
                op = (instr >> 4) & 0x3f;
                a = CreateValue((ushort)(instr >> 10));
                b = default(InternalValue);
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

        private void Skip()
        {
            //Use GetInstr to get the side effect of advancing the PC,
            //but don't let the SP change.
            InternalValue a, b;
            int op;
            ushort temp = SP;
            GetInstr(out op, out a, out b);
            SP = (ushort)temp;
        }

        public void Tick()
        {
            int op;
            InternalValue a, b;
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
                        throw new UnsupportedInstructionException(op);
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
                        temp = (uint)(a.Get() * b.Get());
                        a.Set((ushort)temp);
                        Overflow = (ushort)((temp >> 16) & 0xffff);
                        break;
                    case 0x5: //DIV
                        if (b.Get() == 0)
                        {
                            a.Set(0);
                            Overflow = 0;
                        }
                        else
                        {
                            a.Set((ushort)(a.Get() / b.Get()));
                            Overflow = (ushort)(((a.Get() << 16) / b.Get()) & 0xffff);
                        }
                        break;
                    case 0x6: //MOD
                        if (b.Get() == 0)
                            a.Set(0);
                        else
                            a.Set((ushort)(a.Get() % b.Get()));
                        break;
                    case 0x7: //SHL
                        temp = (uint)(a.Get() << b.Get());
                        Overflow = (ushort)((temp >> 16) & 0xffff);
                        a.Set((ushort)temp);
                        break;
                    case 0x8: //SHR
                        a.Set((ushort)(a.Get() >> b.Get()));
                        Overflow = (ushort)(((a.Get() << 16) >> b.Get()) & 0xffff);
                        break;
                    case 0x9: //AND
                        a.Set((ushort)(a.Get() & b.Get()));
                        break;
                    case 0xa: //BOR
                        a.Set((ushort)(a.Get() | b.Get()));
                        break;
                    case 0xb: //XOR
                        a.Set((ushort)(a.Get() ^ b.Get()));
                        break;
                    case 0xc: //IFE
                        if (!(a.Get() == b.Get()))
                            Skip();
                        break;
                    case 0xd: //IFN
                        if (!(a.Get() != b.Get()))
                            Skip();
                        break;
                    case 0xe: //IFG
                        if (!(a.Get() > b.Get()))
                            Skip();
                        break;
                    case 0xf: //IFB
                        if (!((a.Get() & b.Get()) != 0))
                            Skip();
                        break;
                    default:
                        throw new Exception("Should not get to here.");
                }
            }
        }

        public string Status()
        {
            var sb = new StringBuilder();

            foreach (var name in RegisterNames.Concat(new[] { "PC", "SP", "O" }))
            {
                sb.AppendFormat("{0,5}", name);
            }
            sb.AppendLine();

            const string numberFormat = "{0,5:x}";
            for (int i = 0; i < Registers.Length; i++)
            {
                sb.AppendFormat(numberFormat, Registers[i]);
            }
            sb.AppendFormat(numberFormat, PC);
            sb.AppendFormat(numberFormat, SP);
            sb.AppendFormat(numberFormat, Overflow);

            return sb.ToString();
        }
    }
}
