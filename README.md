DCPU Sharp
----------
[DcpuSharp] is an implementation of the [DCPU-16] processor in C#.  It is first intended to provide a programatic
way for simulating DCPU-16 programs.  These is a EXE project called DcpuTest that runs the sample program.
Here is how you run the CPU:

            ushort[] memory = new ushort[] { /*...*/ };
            var cpu = new Cpu(memory);

            while (true)
            {
                cpu.Tick();
                Console.WriteLine(cpu.Status());
                Thread.Sleep(250);
            }

Dependencies
------------

 - .NET Framework 4.0.  It should also compile against 2.0.

License
-------

DcpuSharp is licensed under the BSD license.


Currently limitations
---------------------

* Does not support the unofficaly instructions or memory mappings for IO.


Plans for future devlopment
---------------------------

 - A nicer front end for loading and executing programs.
 - An API for emiting instructions.  The goal is for the API to feel similar to System.Reflection.Emit.
 - An assembler that uses the emit API.
 - Support for I/O.


  [DcpuSharp]: https://github.com/AustinWise/DcpuSharp
  [DCPU-16]: http://www.0x10c.com/doc/dcpu-16.txt
