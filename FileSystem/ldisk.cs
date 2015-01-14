using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// This contains the 64 blocks of data for all the files in the file system.
    /// </summary>
    class Ldisk
    {
        private Block[] _blocks;

        public Ldisk(Block[] blocks)
        {
            _blocks = blocks;
        }

        public Ldisk()
        {
            _blocks = new Block[64];

            for (var i = 0; i < _blocks.Length; i++)
            {
                SetBlock(new Block(), i);
            }
        }

        public Block ReadBlock(int index)
        {
            return _blocks[index];
        }

        public void SetBlock(Block block, int index)
        {
            _blocks[index] = block;
        }
    }
}
