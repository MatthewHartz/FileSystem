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
    class OLDMemcache
    {
        /*
        private OLDBitmap _bitmap;
        private OLDFileDescriptorData _fileDescriptors;

        public OLDMemcache(Block[] data)
        {
            _bitmap = new OLDBitmap(data[0]);
            _fileDescriptors = new OLDFileDescriptorData(new []
            {
                data[1], data[2], data[3], data[4], data[5], data[6]
            });

            // set first free block (should be block 7) for the directory and initialize 
            // the first descriptor (should be 0) for the directory
            var freeBlock = GetOpenBlock();
            var freeFileDescriptor = _fileDescriptors.GetOpenFileDescriptor();
            _fileDescriptors.SetDescriptorLength(freeFileDescriptor, 0);
            _fileDescriptors.SetDescriptorToBlock(freeFileDescriptor, freeBlock);
        }

        //public void CloseDescriptor(int index)
        //{
        //    _fileDescriptors[index] = null;
        //}

        /// <summary>
        /// Gets an open directory position.  If all too many files already exist, return -1.
        /// </summary>
        /// <returns></returns>
        public int GetOpenDirectoryPosition()
        {
            return -1;
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
         */
    }
}
