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

        public OftFile GetFile(int index)
        {
            try
            {
                return _files[index];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void SetFilePosition(int index, int pos)
        {
            _files[index].position = pos;
        }

        public void CloseFile(int index)
        {
            _files[index] = new OftFile();
        }

        public void OpenFile(int index, Block block, int pos, int fd)
        {
            _files[index] = new OftFile(block, pos, fd);
        }

        public int GetFreeSlot()
        {
            for (int i = 0; i < _files.Length; i++)
            {
                if (_files[i] == null)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    class OftFile
    {
        public Block block { get; set; }
        public int position { get; set; }
        public int index { get; set; }

        public OftFile()
        {
            position = 0;
            index = -1;
            block = new Block();
        }

        public OftFile(Block blockval, int pos, int ind)
        {
            position = pos;
            index = ind;
            block = new Block(blockval.data);
        }
    }
}