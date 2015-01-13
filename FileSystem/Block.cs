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
        private byte[] Data { get; set; }

        public Block()
        {
            Data = new byte[64];
        }
    }
}