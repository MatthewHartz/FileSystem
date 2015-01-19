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
        private FileDescriptor[] _fileDescriptors;

        public Memcache(Block[] ldisk)
        {
            _bitmap = new Bitmap(ldisk[0]);

            // initialze the file descriptors
            _fileDescriptors = InitializeDescriptors(new []
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
        /// Gets the an descriptor map of active blocks
        /// </summary>
        /// <param name="descriptorNumber">The descriptor number.</param>
        /// <returns></returns>
        public List<int> GetDescriptorMap(int descriptorNumber)
        {
            return _fileDescriptors[descriptorNumber].map.Where(x => x != -1).ToList();
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
        /// Get the length of the descriptor.
        /// </summary>
        /// <param name="descriptorNum">The descriptor number.</param>
        public int GetDescriptorLength(int descriptorNum)
        {
            return _fileDescriptors[descriptorNum].length;
        }

        /// <summary>
        /// Initializes the descriptors.
        /// </summary>
        /// <param name="blocks">The blocks.</param>
        /// <returns></returns>
        public FileDescriptor[] InitializeDescriptors(Block[] blocks)
        {
            var filedescriptors = new FileDescriptor[24];
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
                        block[(i * 4)],
                        block[1 + (i * 4)],
                        block[2 + (i * 4)],
                        block[3 + (i * 4)]
                    };

                    fd.length = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                    bytes = new[]
                    {
                        block[4 + (i * 4)],
                        block[5 + (i * 4)],
                        block[6 + (i * 4)],
                        block[7 + (i * 4)]
                    };

                    fd.map[0] = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                    bytes = new[]
                    {
                        block[8 + (i * 4)],
                        block[9 + (i * 4)],
                        block[10 + (i * 4)],
                        block[11 + (i * 4)]
                    };

                    fd.map[1] = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                    bytes = new[]
                    {
                        block[12 + (i * 4)],
                        block[13 + (i * 4)],
                        block[14 + (i * 4)],
                        block[15 + (i * 4)]
                    };

                    fd.map[2] = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                    filedescriptors[index] = fd;
                    index++;
                }
            }

            return filedescriptors;
        }
    }
}
