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
        public int length { get; set; }
        public int[] map { get; set; }

        public FileDescriptor()
        {
            length = 0;
            map = new int[3];
        }

        public FileDescriptor(int l, int[] m)
        {
            length = l;
            map = m;
        }
    }
}
