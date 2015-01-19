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
    public class Block
    {
        public sbyte[] data { get; set; }

        public Block()
        {
            data = new sbyte[64];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = -0x01;
            }
        }

        public Block(sbyte[] d)
        {
            data = new sbyte[64];

            for (var i = 0; i < d.Length; i++)
            {
                data[i] = d[i];
            }
        }
    }
}