using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// This class contains the bitmap and the file descriptors.
    /// The first index of _fileDescriptors is reserved for the file directory.
    /// </summary>
    
    class Memcache
    {
        private Bitmap _bitmap;
        private FileDescriptor[] _fileDescriptors = new FileDescriptor[24];

        public Memcache(Block[] ldisk)
        {
            _bitmap = new Bitmap(ldisk[0]);

            // initialze the file descriptors
            InitializeDescriptors(new []
            {
                ldisk[1], ldisk[2], ldisk[3], ldisk[4], ldisk[5], ldisk[6]
            });
        }

        public void ReleaseDescriptor(int index)
        {
            _fileDescriptors[index] = null;
        }

        public FileDescriptor GetFileDescriptorByIndex(int index)
        {
            return _fileDescriptors[index];
        }

        public void SetFileDescriptorByIndex(int index, FileDescriptor fd)
        {
            _fileDescriptors[index] = fd;
        }

        /// <summary>
        /// Gets the first open file descriptor.  If all file descriptors are full, return -1.
        /// </summary>
        /// <returns></returns>
        public int GetOpenFileDescriptor()
        {
            for (int i = 0; i < _fileDescriptors.Length; i++)
            {
                if (_fileDescriptors[i].length == -1)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Sets the block to descriptor.
        /// </summary>
        /// <param name="descriptorNumber">The descriptor number.</param>
        /// <param name="blockNumber">The block number.</param>
        public bool SetBlockToDescriptor(int descriptorNumber, int blockNumber)
        {
            for (var i = 0; i < 3; i++)
            {
                if (_fileDescriptors[descriptorNumber].map[i] == -1)
                {
                    _fileDescriptors[descriptorNumber].map[i] = blockNumber;
                    _bitmap.SetBit(blockNumber);
                    return true;
                }
             }

            return false;
        }

        /// <summary>
        /// Gets the first open block from the bitmap.  If bitmap is full, return -1.
        /// </summary>
        /// <returns></returns>
        public int GetOpenBlock()
        {
            for (var i = 0; i < 64; i++)
            {
                if (_bitmap.GetBit(i) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the bit map in block form. Only used for saving the bitmap to the ldisk.
        /// </summary>
        /// <returns></returns>
        public Block GetBitMap()
        {
            var block = new Block();
            var counter = 0;

            // iterate over the 8 slots that the bitmap takes
            for (var i = 0; i < 8; i++)
            {
                var tempByte = new sbyte();
                // iterate over each bit in a byte
                for (var j = 0; j < 8; j++)
                {
                    if (_bitmap.GetBit((i*8) + j) != 0)
                        tempByte |= (sbyte)(1 << j);
                    counter++;
                }

                block.data[i] = tempByte;
            }

            return block;
        }

        /// <summary>
        /// Sets the block to 1 in the bitmap
        /// </summary>
        /// <param name="index">The index.</param>
        public void SetBlock(int index)
        {
            _bitmap.SetBit(index);
        }

        /// <summary>
        /// Sets the block to 1 in the bitmap
        /// </summary>
        /// <param name="index">The index.</param>
        public void ClearBlock(int index)
        {
            _bitmap.ClearBit(index);
        }

        /// <summary>
        /// Sets the length of the descriptor.
        /// </summary>
        /// <param name="descriptorNum">The descriptor number.</param>
        /// <param name="length">The length.</param>
        public void SetDescriptorLength(int descriptorNum, int length)
        {
            _fileDescriptors[descriptorNum].length = length;
        }

        /// <summary>
        /// Initializes the descriptors.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        /// <returns></returns>
        public void InitializeDescriptors(Block[] blocks)
        {
            var index = 0;

            foreach (var b in blocks)
            {
                var block = b.data;

                // iterate over the 4 descripts in the block
                for (var i = 0; i < 4; i++)
                {
                    var fd = new FileDescriptor();

                    var bytes = new[]
                    {
                        block[(i * 16)],
                        block[1 + (i * 16)],
                        block[2 + (i * 16)],
                        block[3 + (i * 16)]
                    };

                    fd.length = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                    bytes = new[]
                    {
                        block[4 + (i * 16)],
                        block[5 + (i * 16)],
                        block[6 + (i * 16)],
                        block[7 + (i * 16)]
                    };

                    fd.map[0] = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                    bytes = new[]
                    {
                        block[8 + (i * 16)],
                        block[9 + (i * 16)],
                        block[10 + (i * 16)],
                        block[11 + (i * 16)]
                    };

                    fd.map[1] = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                    bytes = new[]
                    {
                        block[12 + (i * 16)],
                        block[13 + (i * 16)],
                        block[14 + (i * 16)],
                        block[15 + (i * 16)]
                    };

                    fd.map[2] = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                    _fileDescriptors[index] = fd;
                    index++;
                }
            }
        }
    }
}
