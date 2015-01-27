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
            var sb = new StringBuilder(); // Will contain file output
            var stream = new StreamReader("C:\\input.txt");
            String line;

            int handle;

            while ((line = stream.ReadLine()) != null)
            {
                var tokens = line.Split(' ');

                switch (tokens[0].ToLower())
                {
                    case "cr":
                        try
                        {
                            fileSystem.Create(tokens[1] + "\0");
                            sb.AppendLine(tokens[1] + " created\n");
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(String.Format("error: {0}\n", e.Message));
                        }
                        break;
                    case "de":
                        try
                        {
                            fileSystem.Destroy(tokens[1] + "\0");
                            sb.AppendLine(tokens[1] + " destroyed\n");
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(String.Format("error: {0}\n", e.Message));
                        }
                        break;
                    case "op":
                        try
                        {
                            handle = fileSystem.Open(tokens[1] + "\0");
                            sb.AppendLine(String.Format("{0} opened {1}\n", tokens[1], handle));
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(String.Format("error: {0}\n", e.Message));
                        }
                        break;
                    case "cl":
                        try
                        {
                            handle = fileSystem.Close(Convert.ToInt32(tokens[1]));
                            sb.AppendLine(String.Format("{0} closed\n", handle));
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(String.Format("error: {0}\n", e.Message));
                        }
                        
                        break;
                    case "rd":
                        try
                        {
                            var bytes = fileSystem.Read(Convert.ToInt32(tokens[1]), Convert.ToInt32(tokens[2]));
                            sb.AppendLine(Encoding.UTF8.GetString((byte[])(Array)bytes) + "\n");
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(String.Format("error: {0}\n", e.Message));
                        }
                        
                        break;
                    case "wr":
                        try
                        {
                            var bytesWritten = fileSystem.Write(Convert.ToInt32(tokens[1]), Convert.ToChar(tokens[2]), Convert.ToInt32(tokens[3]));
                            sb.AppendLine(String.Format("{0} bytes written\n", bytesWritten));
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(String.Format("error: {0}\n", e.Message));
                        }
                        break;
                    case "sk":
                        try
                        {
                            fileSystem.Lseek(Convert.ToInt32(tokens[1]), Convert.ToInt32(tokens[2]));
                            sb.AppendLine("position is " + Convert.ToInt32(tokens[2]) + "\n");
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(String.Format("error: {0}\n", e.Message));
                        }
                        break;
                    case "dr":
                        try
                        {
                            var files = fileSystem.Directories();
                            sb.AppendLine(files.Aggregate((i, j) => i + ' ' + j) + "\n");
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(String.Format("error: {0}\n", e.Message));
                        }
                        break;
                    case "in":
                        try
                        {
                            if (tokens.Count() == 2)
                            {
                                fileSystem.Init(tokens[1]);
                                sb.AppendLine("disk restored\n");
                            }
                            else if (tokens.Count() == 1)
                            {
                                fileSystem.Init(null);

                                sb.AppendLine("disk initialized\n");
                            }
                            else
                            {
                                sb.AppendLine("error: Invalid init call - in <optional filename>\n");
                            }
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(String.Format("error: {0}\n", e.Message));
                        }
                        break;
                    case "sv":
                        try
                        {
                            fileSystem.Save(tokens[1]);
                            sb.AppendLine("disk saved\n");
                            
                        }
                        catch (Exception e)
                        {
                            sb.AppendLine(String.Format("error: {0}\n", e.Message));
                        }
                        break;
                    case "":
                        sb.AppendLine();
                        break;
                    default:
                        sb.AppendLine("error: invalid operation\n");
                        break;
                }
            } 

            stream.Close();
            File.WriteAllText("C:\\output.txt", sb.ToString());
        }
    }
}
