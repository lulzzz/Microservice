﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xigadee;

namespace Test.Xigadee.Azure.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var pipeline = new MicroservicePipeline("Azure Storage Test");

            pipeline
                .ConfigurationSetFromConsoleArgs(args)
                .AddAzureStorageDataCollector()
                ;

            pipeline.StartWithConsole();
        }
    }
}
