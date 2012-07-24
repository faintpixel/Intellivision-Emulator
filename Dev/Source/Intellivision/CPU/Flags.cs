using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Intellivision.CPU
{
    public class Flags
    {
        public bool Sign = false;
        public bool Carry = false;
        public bool Zero = false;
        public bool Overflow = false;
        public bool InterruptEnable = false;
        public bool DoubleByteData = false;

        public void DEBUG_PRINT_VERBOSE()
        {
            Console.WriteLine("S: " + Sign);
            Console.WriteLine("C: " + Carry);
            Console.WriteLine("Z: " + Zero);
            Console.WriteLine("O: " + Overflow);
            Console.WriteLine("I: " + InterruptEnable);
            Console.WriteLine("D: " + DoubleByteData);
        }

        public void DEBUG_PRINT()
        {
            if (Sign)
                Console.Write("S");
            else
                Console.Write("-");

            if (Carry)
                Console.Write("C");
            else
                Console.Write("-");

            if (Zero)
                Console.Write("Z");
            else
                Console.Write("-");

            if (Overflow)
                Console.Write("O");
            else
                Console.Write("-");

            if (InterruptEnable)
                Console.Write("I");
            else
                Console.Write("-");

            if (DoubleByteData)
                Console.Write("D");
            else
                Console.Write("-");
        }
    }
}
