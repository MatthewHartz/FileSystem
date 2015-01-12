using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// The FileSystem class is the meat and potatoes of this project. 
    /// It will call the file system actions on the ldisk and memcache when called by the driver class.
    /// </summary>
    class FileSystem
    {
        public void Create(string name) {}
        public void Destroy(string name) {}
        public int Open(string name)
        {
            return 0;
        }
        public void Close(int file) { }

        public int Read(int index, Buffer buf, int count)
        {
            return 0;
        }

        public int Write(int index, Buffer buf, int count)
        {
            return 0;
        }
        public void Lseek(int index, int pos) {}

        public List<FSfile> Directory()
        {
            return new List<FSfile>();
        }

        public void Init(string filename) {}
        public void Save(string filename) {}
    }
}
