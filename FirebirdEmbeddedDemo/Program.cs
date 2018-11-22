using FirebirdEmbeddedDemo.Data;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace FirebirdEmbeddedDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello! I will try to get connection string for my FB database!");
            string connectionString = ResolveFbConnectionString();
            Console.WriteLine($"I got this connection string: {connectionString}");

            Console.WriteLine("Connecting to database");

            DemoContext demoContext = new DemoContext(connectionString);

            for (int i = 0; i < 10; i++)
            {
                demoContext.Todos.Add(new Todos()
                {
                    Name = "Blah blah",
                    Description = "describe"
                });
            }
            demoContext.SaveChanges();

            foreach(var todo in demoContext.Todos)
            {
                Console.WriteLine($"ID: {todo.Id}   NAME:{todo.Name}    DESCRIPTION:{todo.Description}");
            }

            Console.WriteLine("Ok");

            while (Console.ReadLine() != "quit")
            {

            }
        }

        private static string ResolveFbConnectionString()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string directory = Path.GetDirectoryName(path);
            string dllPath = "";
            string archPath = "";

            string databasePath = Path.Combine(directory, @"Database", "DEMO.FDB");

            //Make sure you move all relevant files from Firebird compressed release in the coresponding root folder. For example (.dll in Firebird-3.0.4.33054-0_x64_pdb; .so in Firebird-debuginfo-3.0.4.33054-0.i686
            if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    archPath = "Firebird-3.0.4.33054-0_x64";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    archPath = "Firebird-3.0.4.33054-0.amd64";
                else
                    throw new NotSupportedException("Unsupported platform");
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.X86)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    archPath = "Firebird-3.0.4.33054-0_Win32";
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    throw new NotSupportedException("Unsupported by dotnet core platform");
                else
                    throw new NotSupportedException("Unsupported platform");
            }
            else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm)
            {
                archPath = "Firebird-withDebugInfo-3.0.4.33054-0.arm";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                dllPath = Path.Combine(directory, "Firebird", archPath, "libfbclient.so");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                dllPath = Path.Combine(directory, "Firebird", archPath, "fbclient.dll");
            }
            //"ServerType=1;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;User=SYSDBA;Password=masterkey;Database=" + databasePath + "; clientlibrary=" + dllPath;
            var connectionString = $"ServerType=1;Dialect=3;User=SYSDBA;Password=masterkey;Database={databasePath}; clientlibrary={dllPath}";
            return connectionString;
        }
    }
}
