﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

// based test code -> https://github.com/mbdavid/LiteDB-Perf

namespace TestPerfLiteDB
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest("LiteDB: default", new LiteDB_Test(5000, null, new FileOptions { Journal = true, FileMode = FileMode.Shared }));
            //RunTest("LiteDB: encrypted", new LiteDB_Test(5000, "mypass", new FileOptions { Journal = true, FileMode = FileMode.Shared }));
            //RunTest("LiteDB: exclusive no journal", new LiteDB_Test(5000, null, new FileOptions { Journal = false, FileMode = FileMode.Exclusive }));
            RunTest("LiteDB: in-memory", new LiteDB_Test(5000));

            RunTest("SQLite: default", new SQLite_Test(5000, null, true));
            //RunTest("SQLite: encrypted", new SQLite_Test(5000, "mypass", true));
            //RunTest("SQLite: no journal", new SQLite_Test(5000, null, false));
            RunTest("SQLite: in-memory", new SQLite_Test(5000, null, false, true));

            // RunTest("RavenDB: in-memory", new RavenDB_Test(5000, true));

            RunTest("Dictionary", new Dictionary_Test(5000));
            RunTest("ConcurrentDictionary", new ConcurrentDictionary_Test(5000));

            RunTest("MasterMemory: Plain", new MasterMemory_Test(5000));
            RunTest("MasterMemory: Loaded", new MasterMemoryDatabase_Test(5000));

            Console.ReadKey();
        }

        static void RunTest(string name, ITest test)
        {
            var title = name + " - " + test.Count + " records";
            Console.WriteLine(title);
            Console.WriteLine("=".PadLeft(title.Length, '='));

            test.Prepare();

            test.Run("Insert", test.Insert, true);
            test.Run("Bulk", test.Bulk, true);
            test.Run("CreateIndex", test.CreateIndex,true);
            test.Run("Query", test.Query, false);
            test.Run("Query", test.Query, false);
            test.Run("Query", test.Query, false);
            test.Run("Query", test.Query, false);
            
            try
            {
                 Console.WriteLine("FileLength     : " + Math.Round((double)test.FileLength / (double)1024, 2).ToString().PadLeft(5, ' ') + " kb");
            }
            catch (System.IO.FileNotFoundException)
            {
            }

            test.Dispose();

            Console.WriteLine();

        }
    }
}