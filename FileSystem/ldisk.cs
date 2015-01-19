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
        private static Ldisk _instance;

        public static Ldisk Instance
        {
            get { return _instance ?? (_instance = new Ldisk()); }
        }

        public Ldisk(Block[] blocks)
        {
            _blocks = blocks;
        }

        public Ldisk()
        {
            _blocks = new Block[64];

            // set the bitmap to //0x0000000000007F
            var block = new sbyte[64];
            block[0] = 0x7F;
            SetBlock(new Block(block), 0);

            // sets the rest of the ldisk to 0
            for (var i = 1; i < _blocks.Length; i++)
            {
                SetBlock(new Block(), i);
            }
        }

        public Block ReadBlock(int index)
        {
            return new Block(_blocks[index].data);
        }

        public void SetBlock(Block block, int index)
        {
            _blocks[index] = block;
        }
    }
}
