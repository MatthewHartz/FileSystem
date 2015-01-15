using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// This class is for the file descriptors in the project
    /// </summary>
    class FileDescriptor
    {
        private int _length;
        private int[] _map;

        public FileDescriptor()
        {
            _length = 0;
            _map = new int[3];
        }

        public FileDescriptor(int length, int[] map)
        {
            _length = length;
            _map = map;
        }

        public static FileDescriptor[] InitializeDescriptors(Block[] blocks)
        {
            var filedescriptors = new FileDescriptor[24];

            foreach (var block in blocks)
            {
                for (int i = 0; i < 64; i++)
                {
                    
                }
            }

            return new FileDescriptor[24];
        }
    }
}
