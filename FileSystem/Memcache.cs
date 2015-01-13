using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// This class contains the bitmap and the file descriptors
    /// </summary>
    class Memcache
    {
        private Bitmap bitmap;
        private FileDescriptor[] fileDescriptors;

        public Memcache()
        {
            bitmap = new Bitmap();
            fileDescriptors = new FileDescriptor[24];
        }
    }
}
