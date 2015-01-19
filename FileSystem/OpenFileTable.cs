using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        //private Ldisk _ldisk = Ldisk.Instance;
        private readonly OftFile[] _files;

        public OpenFileTable()
        {
            _files = new OftFile[4];
        }

        public int AddFile(Block block, int pos, int index)
        {
            for (var i = 0; i < _files.Length; i++)
            {
                _files[i] = new OftFile(block, pos, index);
                return i;
            }

            return -1;
        }

        public OftFile GetFile(int index)
        {
            return _files[index];
        }

        public void SetFilePosition(int index, int pos)
        {
            _files[index].position = pos;
        }

        public void UpdateFile(int index, OftFile file)
        {
            _files[index] = file;
        }
    }

    class OftFile
    {
        public Block block { get; set; }
        public int position { get; set; }
        public int index { get; set; }

        public OftFile() { }

        public OftFile(Block blockval, int pos, int ind)
        {
            position = pos;
            index = ind;
            block = new Block(blockval.data);
        }
    }
}