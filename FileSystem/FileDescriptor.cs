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
        private int length;
        private int[] map;

        public FileDescriptor()
        {
            length = 0;
            map = new int[3];
        }
    }
}
