using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// This class represents the Open File Table. This is a cache that stores information 
    /// on all open files currently in the program. By default the directory file
    /// is open throughout the program.
    /// </summary>
    class OpenFileTable
    {
        private OftFile[] files;

        public OpenFileTable()
        {
            files = new OftFile[3];
        }
    }

    class OftFile
    {
        private Block block;
        private int position;
        private int index;
    }
}
