﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Austin.DcpuSharp;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace Austin.DcpuTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ushort> mem = new List<ushort>();
            foreach (var line in Properties.Resources.ExampleProgram.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Split(':')[1]))
            {
                foreach (var b in line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mem.Add(ushort.Parse(b, NumberStyles.HexNumber));
                }
            }

            var cpu = new Cpu(mem.ToArray());

            Console.WriteLine(cpu.Status());
            Console.WriteLine();

            int ticks = 0;
            var st = Stopwatch.StartNew();
            while (ticks < 10000000)
            {
                cpu.Tick();
                Console.WriteLine(cpu.Status());
                Console.WriteLine();
                Thread.Sleep(250);
                ticks++;
            }
            st.Stop();

            //Console.WriteLine(st.ElapsedMilliseconds);

            //Console.WriteLine(ticks / (double)st.ElapsedMilliseconds * 1000);
        }
    }
}
