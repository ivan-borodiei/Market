﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new Client();

            client.Process();

            Console.ReadLine();
        }
    }
}
