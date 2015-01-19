using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    class FileDirectory
    {
        private Ldisk _ldisk = Ldisk.Instance;
        private List<DirectoryFile> _files; 

        public static bool OpenFileExists(Block block)
        {
            var index = 0;
            var b = block.data;
            while (index < 64)
            {
                var bytes = new[]
                    {
                        b[index],
                        b[index + 1],
                        b[index + 2],
                        b[index + 3]
                    };
                var value = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                if (value == -1)
                {
                    /*
                    return new DirectoryFile(
                    {
                        name = new char[4] { 'a', 'b', 'c', 'd'},
                        descriptorIndex = 4
                    };
                     */
                     
                }
            }

            return false;
        }
    }

        /// <summary>
    /// This is the data structure for a file.
    /// </summary>
    public class DirectoryFile
    {
        public char[] name;
        public int descriptorIndex;

        public DirectoryFile() { }

        public DirectoryFile(sbyte[] n, sbyte[] d)
        {
            for (int i = 0; i < 4; i++)
            {
                name[i] = (char)n[i];
            }

            descriptorIndex = Convert.ToInt32(d);
        }

        public DirectoryFile(char[] n, int index)
        {
            name = n;
            descriptorIndex = index;
        }
    }
}
