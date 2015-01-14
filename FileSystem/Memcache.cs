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
            _fileDescriptors[0] = new FileDescriptor();
        }

        public FileDescriptor GetDescriptor(int index)
        {
            return _fileDescriptors[index];
        }

        public void CloseDescriptor(int index)
        {
            _fileDescriptors[index] = null;
        }

        public int GetOpenFileDescriptor()
        {
            for (int i = 0; i < _fileDescriptors.Length; i++)
            {
                if (_fileDescriptors[i] != null)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
