using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// This class is for the file descriptors in the project
    /// </summary>
    class OLDFileDescriptorData
    {/*
        private Block[] _fileDescriptors;

        public OLDFileDescriptorData(Block[] data)
        {
            _fileDescriptors = data;
        }

        /// <summary>
        /// Gets the first open file descriptor.  If all file descriptors are full, return -1.
        /// File descriptors are 16 bytes in length. Therefore, check the first 4 bytes to see
        /// if the value is -1. If it is not, skip over the 16 bytes and check the following.
        /// </summary>
        /// <returns></returns>
        public int GetOpenFileDescriptor()
        {
            var index = 0;
            while (GetDescriptorLength(index) != -1) { index++; }

            return (index == 24) ? -1 : index;
        }

        /// <summary>
        /// Sets the descriptor to point to another block. Returns -1 if descriptor already uses 3 blocks
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="ldiskBlock">The block number.</param>
        /// <returns></returns>
        public bool SetDescriptorToBlock(int index, int ldiskBlock)
        {
            var blockNumber = index/4; // gets which block to get data from
            var block = _fileDescriptors[blockNumber].GetBlock();
            var blockPos = ( index % 4 ) * 16;

            for (var i = 1; i < 4; i++)
            {
                var bytes = new[]
                    {
                        block[blockPos + (i * 4)],
                        block[(blockPos + 1) + (i * 4)],
                        block[(blockPos + 2) + (i * 4)],
                        block[(blockPos + 3) + (i * 4)]
                    };

                var value = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                if (value == -1)
                {
                    var ldb = BitConverter.GetBytes(ldiskBlock);

                    block[blockPos + (i * 4)] = (sbyte)ldb[0];
                    block[(blockPos + 1) + (i * 4)] = (sbyte)ldb[1];
                    block[(blockPos + 2) + (i * 4)] = (sbyte)ldb[2];
                    block[(blockPos + 3) + (i * 4)] = (sbyte)ldb[3];

                    _fileDescriptors[blockNumber].SetBlock(block);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the length of the descriptor.
        /// </summary>
        /// <param name="descriptorNum">The descriptors number</param>
        /// <returns></returns>
        public int GetDescriptorLength(int descriptorNum)
        {
            var blockNumber = descriptorNum / 4; // gets which block to get data from
            var block = _fileDescriptors[blockNumber].GetBlock();
            var blockPos = (descriptorNum % 4) * 16;

            var bytes = new[]
            {
                block[blockPos],
                block[blockPos + 1],
                block[blockPos + 2],
                block[blockPos + 3]
            };

            return BitConverter.ToInt32((byte[])(Array)bytes, 0);
        }

        /// <summary>
        /// Sets the length of the descriptor.
        /// </summary>
        /// <param name="index">The descriptors number.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public void SetDescriptorLength(int descriptorNum, int length)
        {
            var blockNumber = descriptorNum / 4; // gets which block to get data from
            var block = _fileDescriptors[blockNumber].GetBlock();
            var blockPos = (descriptorNum % 4) * 16;

            var bytes = BitConverter.GetBytes(length);

            block[blockPos] = (sbyte)bytes[0];
            block[blockPos + 1] = (sbyte)bytes[1];
            block[blockPos + 2] = (sbyte)bytes[2];
            block[blockPos + 3] = (sbyte)bytes[3];

            _fileDescriptors[blockNumber].SetBlock(block);
        }
      */
    }
}
