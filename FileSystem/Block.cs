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
        private byte[] _data { get; set; }

        public Block()
        {
            _data = new byte[64];

            //foreach (var d in _data)
            //{
            //    d = 0;
            //}
        }

        public Block(byte[] data)
        {
            _data = data;
        }
    }
}