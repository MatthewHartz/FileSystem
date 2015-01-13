using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// The FileSystem Singleton is the meat and potatoes of this project.
    /// It will call the file system actions on the ldisk and memcache when called by the driver class.
    /// </summary>
    class FileSystem
    {
        private Ldisk ldisk;
        private OpenFileTable oft;
        private Memcache memcache;
        private static FileSystem instance;

        private FileSystem() {}

        public static FileSystem Instance
        {
            get { return instance ?? (instance = new FileSystem()); }
        }

        /// <summary>
        /// Creates the specified file.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        public void Create(string name) {}

        /// <summary>
        /// Destroys the specified file.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Destroy(string name) {}

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public int Open(string name)
        {
            return 0;
        }
        /// <summary>
        /// Closes the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void Close(int file) { }

        /// <summary>
        /// Reads count numbers from the specified file at location index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="buf">The buf.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public int Read(int index, Buffer buf, int count)
        {
            return 0;
        }

        /// <summary>
        /// Writes the character count times to the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="character">The character.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public int Write(int index, char character, int count)
        {
            return 0;
        }
        /// <summary>
        /// Seeks to the specified position in the file.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="pos">The position.</param>
        public void Lseek(int index, int pos) {}

        /// <summary>
        /// Prints out the list of directories
        /// </summary>
        /// <returns></returns>
        public List<FSfile> Directories()
        {
            return new List<FSfile>();
        }

        /// <summary>
        /// If filename is null, create a new file system. If the specified
        /// file exists, initialize the file system using that file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Init(string filename)
        {
            if (String.IsNullOrEmpty(filename))
            {
                ldisk = new Ldisk();
                oft = new OpenFileTable();
                memcache = new Memcache();

                Console.WriteLine("disk initialized");
            }
            else
            {
                Console.WriteLine("disk restored");
            }
        }

        /// <summary>
        /// Saves the current state of the file system to the file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Save(string filename) {}
    }
}
