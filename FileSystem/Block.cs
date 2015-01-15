using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// This class represents the blocks of memory inside the ldisk.
    /// Each block is 64 bytes
    /// </summary>
    class Block
    {
        private sbyte[] _data;

        public Block()
        {
            _data = new sbyte[64];
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i] = -0x01;
            }
        }

        public Block(sbyte[] data)
        {
            _data = data;
        }

        public sbyte[] GetBlock()
        {
            return _data;    
        }

        public void SetBlock(sbyte[] block)
        {
            _data = block;
        }
    }
}