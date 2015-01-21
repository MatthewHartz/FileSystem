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
                            fileSystem.Create(tokens[1] + "\0");
                            Console.WriteLine(tokens[1] + " created");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
                        }
                        break;
                    case "de":
                        try
                        {
                            fileSystem.Destroy(tokens[1] + "\0");
                            Console.WriteLine(tokens[1] + " destroyed");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
                        }
                        break;
                    case "op":
                        try
                        {
                            handle = fileSystem.Open(tokens[1] + "\0");
                            Console.WriteLine("{0} opened {1}", tokens[1], handle);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
                        }
                        break;
                    case "cl":
                        try
                        {
                            handle = fileSystem.Close(Convert.ToInt32(tokens[1]));
                            Console.WriteLine("{0} closed {1}", handle);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
                        }
                        
                        break;
                    case "rd":
                        try
                        {
                            var bytes = fileSystem.Read(Convert.ToInt32(tokens[1]), Convert.ToInt32(tokens[2]));
                            Console.WriteLine(Encoding.UTF8.GetString((byte[])(Array)bytes));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
                        }
                        
                        break;
                    case "wr":
                        try
                        {
                            var bytesWritten = fileSystem.Write(Convert.ToInt32(tokens[1]), Convert.ToChar(tokens[2]), Convert.ToInt32(tokens[3]));
                            Console.WriteLine("{0} bytes written", bytesWritten);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
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
                            Console.WriteLine("Error: {0}", e.Message);
                        }
                        break;
                    case "dr":
                        try
                        {
                            var files = fileSystem.Directories();
                            Console.WriteLine(files.Aggregate((i, j) => i + ' ' + j));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
                        }
                        break;
                    case "in":
                        try
                        {
                            if (tokens.Count() == 2)
                            {
                                fileSystem.Init(tokens[1]);
                                Console.WriteLine("Disk restored");
                            }
                            else if (tokens.Count() == 1)
                            {
                                fileSystem.Init(null);
                                Console.WriteLine("Disk initialized");
                            }
                            else
                            {
                                Console.WriteLine("Error: Invalid init call - in <optional filename>");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
                        }
                        break;
                    case "sv":
                        try
                        {
                            fileSystem.Save(tokens[1]);
                            Console.WriteLine("Disk saved");
                            
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error: {0}", e.Message);
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
