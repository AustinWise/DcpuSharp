using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Austin.DcpuSharp
{
    internal class Fixup
    {
        public readonly Label Label;
        public readonly int Position;

        public Fixup(Label lbl, int pos)
        {
            this.Label = lbl;
            this.Position = pos;
        }
    }
}
