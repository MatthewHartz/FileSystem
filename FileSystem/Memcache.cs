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

        public Memcache()
        {
            _bitmap = new Bitmap();
            _fileDescriptors = new FileDescriptor[24];

            // initialize directory descriptor
            var freeBlock = GetOpenBlock();
            _fileDescriptors[0] = new FileDescriptor(0, new []
            {
                freeBlock, -1, -1
            });

            // Set the bit for the first directory block
            _bitmap.SetBit(7);
        }

        public FileDescriptor GetDirectoryDescriptor()
        {
            return _fileDescriptors[0];
        }

        public FileDescriptor GetDescriptor(int index)
        {
            return _fileDescriptors[index];
        }

        public void CloseDescriptor(int index)
        {
            _fileDescriptors[index] = null;
        }

        /// <summary>
        /// Gets the first open file descriptor.  If all file descriptors are full, return -1.
        /// </summary>
        /// <returns></returns>
        public int GetOpenFileDescriptor()
        {
            for (int i = 0; i < _fileDescriptors.Length; i++)
            {
                if (_fileDescriptors[i] == null)
                {
                    return i;
                }
            }

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
    }
}
