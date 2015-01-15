using System;

namespace FileSystem
{
    /// <summary>
    /// This class represents the bitmap for all the open blocks
    /// </summary>
    class OLDBitmap
    {
        /*
        private Block _map;
        private ulong _bitmask;

        public OLDBitmap(Block bitStream)
        {
            _map = bitStream;
            _bitmask = 0x0000000000000001;
        }

        /// <summary>
        /// Gets the bit indicated by index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public int GetBit(int index)
        {
            var map = BitConverter.ToUInt64((byte[])(Array)_map.GetBlock(), 0);

            return (map & (_bitmask << index)) == 0 ? 0 : 1;
        }

        /// <summary>
        /// Sets the bit indicated by index to the value of bit.
        /// </summary>
        /// <param name="bit">The bit.</param>
        /// <param name="index">The index.</param>
        public void SetBit(int index)
        {
            var map = BitConverter.ToUInt64((byte[])(Array)_map.GetBlock(), 0);
            map |= (_bitmask << index);

            _map.SetBlock((sbyte[]) (Array)BitConverter.GetBytes(map));
        }

        /// <summary>
        /// Clears the bit indicated by index to the value of bit.
        /// </summary>
        /// <param name="bit">The bit.</param>
        /// <param name="index">The index.</param>
        public void ClearBit(int index)
        {
            var map = BitConverter.ToUInt64((byte[])(Array)_map.GetBlock(), 0);
            map &= ~(_bitmask << index);

            _map.SetBlock((sbyte[])(Array)BitConverter.GetBytes(map));
        }
         */
    }
}