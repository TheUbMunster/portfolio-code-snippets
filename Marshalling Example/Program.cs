using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Marshal_Reciever
{
    class Program
    {
        private static unsafe List<double> argm = new List<double>();
        private static unsafe double* r;
        private static unsafe double* d1;
        private static unsafe double* d2;
        [DllImport("Marshalling.dll", CharSet = CharSet.Unicode)]
        public static unsafe extern void Add(double* result, double* a, double* b);
        [DllImport("Marshalling.dll", CharSet = CharSet.Unicode)]
        public static unsafe extern double* GetDoublePtr();
        [DllImport("Marshalling.dll", CharSet = CharSet.Unicode)]
        public static unsafe extern void DeleteDoublePtr(double* toDelete);
        static void Main(string[] args)
        {
            unsafe
            {
                d1 = GetDoublePtr();
                d2 = GetDoublePtr();
                r = GetDoublePtr();
            }
        toptop:
            do
            {
                Console.WriteLine("Type " + ((argm.Count < 1) ? "any" : "another") + " number between negative one million and one million.");
                if (!double.TryParse(Console.ReadLine(), out double arg))
                {
                    Console.WriteLine("Whoops! that's not a number; try again.");
                }
                else
                {
                    if (arg < 1000000d && arg > -1000000d)
                    {
                        argm.Add(arg);
                    }
                    else
                    {
                        Console.WriteLine("Whoops, that number's too big/small! try again.");
                    }
                }
            } while (argm.Count < 2);
            unsafe
            {
                *d1 = argm[0];
                *d2 = argm[1];
                Add(r, d1, d2);
                Console.WriteLine("Here is the sum of your values: " + *r + System.Environment.NewLine + "(Values were marshalled over to C++, calculated, then returned to C#).");
            }
            argm.Clear();
            Console.WriteLine("Would you like to calculate another number? (Y/N).");
            if (Console.ReadLine().ToLower() == "y")
            {
                goto toptop;
            }
            else
            {
                unsafe
                {
                    DeleteDoublePtr(r);
                    DeleteDoublePtr(d1);
                    DeleteDoublePtr(d2);
                }
            }
        }
    }
}