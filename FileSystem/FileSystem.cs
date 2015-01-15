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
        private Ldisk _ldisk;
        private OpenFileTable _oft;
        private Memcache _memcache;
        private static FileSystem _instance;

        private FileSystem() {}

        public static FileSystem Instance
        {
            get { return _instance ?? (_instance = new FileSystem()); }
        }

        /// <summary>
        /// Creates the specified file.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        public void Create(string name)
        {
            try
            {
                // Find open file descriptor
                var descriptor = _memcache.GetOpenFileDescriptor();

                if (descriptor == -1)
                {
                    Console.WriteLine("No empty file descriptors");
                    return;
                }
                    

                // get the list of blocks used by directory descriptor
                var blocks = _memcache.GetDescriptorMap(0);

                foreach (var block in blocks)
                {
                    var data = _ldisk.ReadBlock(block);

                    if (FSfile.OpenFileExists(block))
                    {
                        FSfile.SetFile(name, descriptor);
                        _memcache.SetDescriptorLength(descriptor, 0);
                    }
                    //if
                }

                // If a open file was not found and a new block can be added, add it.
                if (blocks.Count != 3)
                {
                    var newblock = _memcache.GetOpenBlock();
                    _memcache.SetBlockToDescriptor(descriptor, newblock);
                    var data = _ldisk.ReadBlock(newblock);
                    //file = FSfile.GetOpenFile(data);
                }

                // Have both descriptor and file, fill both entries
                /*
                if (file != null)
                {
                    var length = _memcache.GetDescriptorLength(0);
                    _memcache.SetDescriptorLength(0, length + 8);
                    Console.WriteLine("{0} created", name);
                }
                else
                {
                    Console.WriteLine("No empty directory files");
                }
                 */
                
                
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine("Disk not initialized");
            }

        }

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
            /*
             * search directory to find index of file descriptor (i)
             * allocate a free OFT entry (reuse deleted entries)
             * fill in current position (0) and file descriptor index (i)
             * read block 0 of file into the r/w buffer (read-ahead)
             * return OFT index (j) (or return error)
             * consider adding a file length field (to simplify checking)
             */

            return 0;
        }

        /// <summary>
        /// Closes the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void Close(int file)
        {
            /*
             * write buffer to disk
             * update file length in descriptor
             * free OFT entry
             * return status
             */


        }

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
                _ldisk = new Ldisk();

                var blocks = new[]
                {
                    _ldisk.ReadBlock(0),
                    _ldisk.ReadBlock(1),
                    _ldisk.ReadBlock(2),
                    _ldisk.ReadBlock(3),
                    _ldisk.ReadBlock(4),
                    _ldisk.ReadBlock(5),
                    _ldisk.ReadBlock(6),
                };
                
                _memcache = new Memcache(blocks);
                _oft = new OpenFileTable();

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

        private int GetDirectoryEntry()
        {
            //var block = _ldisk.ReadBlock();

            return -1;
        }
    }
}
