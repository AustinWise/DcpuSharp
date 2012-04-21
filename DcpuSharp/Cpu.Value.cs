using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DcpuSharp
{
    partial class Cpu
    {
        private InternalValue CreateValue(ushort ndx)
        {
            if (ndx < 0x08)
                return new InternalValue(Registers, ndx);
            if (ndx < 0x10)
                return new InternalValue(Memory, Registers[ndx - 0x8]);
            if (ndx < 0x18)
                return new InternalValue(Memory, (ushort)(Memory[PC++] + Registers[ndx - 0x10]));
            switch (ndx)
            {
                case 0x18:
                    return new InternalValue(Memory, SP++); //POP
                case 0x19:
                    return new InternalValue(Memory, SP); //PEEK
                case 0x1a:
                    return new InternalValue(Memory, --SP); //PUSH
                case 0x1b:
                    return new InternalValue(SaveLocation.SP, this);
                case 0x1c:
                    return new InternalValue(SaveLocation.PC, this);
                case 0x1d:
                    return new InternalValue(SaveLocation.Overflow, this);
                case 0x1e:
                    return new InternalValue(Memory, Memory[PC++]);
                case 0x1f:
                    return new InternalValue(Memory[PC++]);
                default:
                    break;
            }
            if (ndx < 0x40)
                return new InternalValue((ushort)(ndx - 0x20));
            throw new ArgumentOutOfRangeException();
        }

        enum SaveLocation
        {
            None = 0,
            Literal,
            Memory,
            SP,
            PC,
            Overflow,
        }

        struct InternalValue
        {
            public InternalValue(ushort[] buffer, ushort index)
            {
                this.Buffer = buffer;
                this.Index = index;
                this.SaveLoc = SaveLocation.Memory;
                this.Literal = 0;
                this.MyCpu = null;
            }

            public InternalValue(SaveLocation loc, Cpu cpu)
            {
                this.Buffer = null;
                this.Index = 0;
                this.SaveLoc = loc;
                this.Literal = 0;
                this.MyCpu = cpu;
            }

            public InternalValue(ushort lit)
            {
                this.Buffer = null;
                this.Index = 0;
                this.SaveLoc = SaveLocation.Literal;
                this.Literal = lit;
                this.MyCpu = null;
            }

            //Save loc, literal, and index could be combined into one int.
            //That might speed up things or lower memory useage
            private SaveLocation SaveLoc;
            private ushort[] Buffer;
            private ushort Index;
            private ushort Literal;
            private Cpu MyCpu;

            public ushort Get()
            {
                if (SaveLoc == SaveLocation.Memory)
                    return Buffer[Index];
                else if (SaveLoc == SaveLocation.Literal)
                    return Literal;
                else if (SaveLoc == SaveLocation.SP)
                    return MyCpu.SP;
                else if (SaveLoc == SaveLocation.PC)
                    return MyCpu.PC;
                else if (SaveLoc == SaveLocation.Overflow)
                    return MyCpu.Overflow;
                throw new NotSupportedException(SaveLoc.ToString());
            }

            public void Set(ushort val)
            {
                if (SaveLoc == SaveLocation.Memory)
                    Buffer[Index] = val;
                else if (SaveLoc == SaveLocation.Literal)
                    return; //ignore attempts to set literals
                else if (SaveLoc == SaveLocation.SP)
                    MyCpu.SP = val;
                else if (SaveLoc == SaveLocation.PC)
                    MyCpu.PC = val;
                else if (SaveLoc == SaveLocation.Overflow)
                    MyCpu.Overflow = val;
                else
                    throw new NotSupportedException(SaveLoc.ToString());
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                if (SaveLoc == SaveLocation.Memory)
                {
                    if (Buffer.Length == 8)
                    {
                        sb.Append("Register: ");
                        sb.Append(RegisterNames[Index]);
                    }
                    else
                    {
                        sb.AppendFormat("Memory: 0x{0:x}", Index);
                    }
                }
                else
                {
                    sb.Append(SaveLoc.ToString());
                }

                if (SaveLoc != SaveLocation.None)
                    sb.AppendFormat(" Value: 0x{0:x}", Get());

                return sb.ToString();
            }
        }
    }
}
