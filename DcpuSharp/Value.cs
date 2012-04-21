using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DcpuSharp
{
    public abstract class Value
    {
        public static readonly Value Pop = new ConstValue(0x18);
        public static readonly Value Peek = new ConstValue(0x19);
        public static readonly Value Push = new ConstValue(0x1a);
        private class ConstValue : Value
        {
            private byte mEncoding;
            public ConstValue(byte encoding)
            {
                this.mEncoding = encoding;
            }

            public override byte Encode()
            {
                return mEncoding;
            }

            public override ushort? GetNextWord()
            {
                return null;
            }
        }

        public abstract byte Encode();

        public abstract ushort? GetNextWord();

        public static implicit operator Value(ushort val)
        {
            return new LiteralValue(val);
        }

        public static implicit operator Value(Register reg)
        {
            return new RegisterValue(reg);
        }

        protected static void CheckRegisterArgument(Register reg)
        {
            if ((byte)reg > 0x7)
                throw new ArgumentOutOfRangeException();
        }
    }

    public class LiteralValue : Value
    {
        private ushort mVal;

        public LiteralValue(ushort val)
        {
            this.mVal = val;
        }

        public override byte Encode()
        {
            if (mVal < 0x20)
                return (byte)(mVal + 0x20);
            return 0x1f;
        }

        public override ushort? GetNextWord()
        {
            if (mVal < 0x20)
                return null;
            return mVal;
        }
    }

    public class RegisterValue : Value
    {
        Register mReg;
        public RegisterValue(Register reg)
        {
            this.mReg = reg;
        }

        public override byte Encode()
        {
            return (byte)mReg;
        }

        public override ushort? GetNextWord()
        {
            return null;
        }
    }

    public class RegisterIndirect : Value
    {
        private Register mReg;
        public RegisterIndirect(Register reg)
        {
            CheckRegisterArgument(reg);
            this.mReg = reg;
        }

        public override byte Encode()
        {
            return (byte)(0x8 + (byte)mReg);
        }

        public override ushort? GetNextWord()
        {
            return null;
        }
    }

    public class RegisterPlusOffsetIndirectValue : Value
    {
        private Register mReg;
        private ushort mConst;

        public RegisterPlusOffsetIndirectValue(Register reg, ushort offset)
        {
            CheckRegisterArgument(reg);
            mReg = reg;
            mConst = offset;
        }

        public override byte Encode()
        {
            return (byte)(0x10 + (byte)mReg);
        }

        public override ushort? GetNextWord()
        {
            return mConst;
        }
    }

    //TODO: better name
    public class MemoryIndirectValue : Value
    {
        private ushort mAddress;
        public MemoryIndirectValue(ushort address)
        {
            this.mAddress = address;
        }

        public override byte Encode()
        {
            return 0x1e;
        }

        public override ushort? GetNextWord()
        {
            return mAddress;
        }
    }
}
