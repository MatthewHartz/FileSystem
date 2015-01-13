using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// This class represents the bitmap for all the open blocks
    /// </summary>
    class Bitmap
    {
        private ulong _map; // ulong in C# is 64 bits
        private ulong _bitmask = 0x0000000000000001;

        public Bitmap()
        {
            _map = 0x0000000000000000;
        }

        public Bitmap(ulong bitStream)
        {
            _map = bitStream;
        }

        /// <summary>
        /// Gets the bit indicated by index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public ulong GetBit(int index)
        {
            return (_map & (_bitmask << index));
        }

        /// <summary>
        /// Sets the bit indicated by index to the value of bit.
        /// </summary>
        /// <param name="bit">The bit.</param>
        /// <param name="index">The index.</param>
        public void SetBit(int bit, int index)
        {
            _map &= ~(_bitmask << index);
        }
    }
}