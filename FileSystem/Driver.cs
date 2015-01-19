using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// The Driver class reads input line by line and executes the correct function based on input.
    /// </summary>
    class Driver
    {
        static void Main(string[] args)
        {
            var fileSystem = FileSystem.Instance;

            var userEntry = "";
            int handle;

            while (true)
            {
                userEntry = Console.ReadLine();               
                var tokens = userEntry.Split(' ');

                switch (tokens[0].ToLower())
                {
                    case "cr":
                        try
                        {
                            if (fileSystem.Create(tokens[1] + "\0"))
                                Console.WriteLine(tokens[1] + " created");                         
                        }
                        catch (Exception e) { }
                        break;
                    case "de":
                        try
                        {
                            if (fileSystem.Destroy(tokens[1] + "\0"))
                                Console.WriteLine(tokens[1] + " destroyed");
                        }
                        catch (Exception e) { }
                        break;
                    case "op":
                        handle = fileSystem.Open(tokens[1] + "\0");
                        if (handle != -1)
                        {
                            Console.WriteLine("{0} opened {1}", tokens[1], handle);
                        }
                        break;
                    case "cl":
                        try
                        {
                            fileSystem.Close(Convert.ToInt32(tokens[1]));
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("Parameter is not a valid integer");
                        }
                        
                        break;
                    case "rd":
                        try
                        {
                            fileSystem.Read(Convert.ToInt32(tokens[1]), Convert.ToInt32(tokens[2]));
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("Invalid read call: rd <index> <count>");
                        }
                        
                        break;
                    case "wr":
                        try
                        {
                            var bytesWritten = fileSystem.Write(Convert.ToInt32(tokens[1]), Convert.ToChar(tokens[2]), Convert.ToInt32(tokens[3]));
                            Console.WriteLine("{0} bytes written", bytesWritten);
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("Invalid read call: rd <index> <char> <count>");
                        }
                        break;
                    case "sk":
                        try
                        {
                            fileSystem.Lseek(Convert.ToInt32(tokens[1]), Convert.ToInt32(tokens[2]));
                            Console.WriteLine("Position is " + Convert.ToInt32(tokens[2]));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error seeking");
                        }
                        break;
                    case "dr":
                        var files = fileSystem.Directories();

                        try
                        {
                            Console.WriteLine(files.Aggregate((i, j) => i + ' ' + j));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("No files exist");
                        }
                        break;
                    case "in":
                        if (tokens.Count() == 2)
                        {
                            fileSystem.Init(tokens[1]);
                        }
                        else if (tokens.Count() == 1)
                        {
                            fileSystem.Init(null);
                        }
                        else
                        {
                            Console.WriteLine("Invalid init call: in <optional filename>");
                        }
                        break;
                    case "sv":
                        if (fileSystem.Save(tokens[1]))
                        {
                            Console.WriteLine("Disk saved");
                        }
                        break;
                    default:
                        Console.WriteLine("Error: invalid operation");
                        break;
                }
            } 
        }
    }
}
