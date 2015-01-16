using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        // const variables
        private const int DirectoryFileDescriptor = 0;
        private const int MaxBlockLength = 64;

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
            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                Console.WriteLine("Disk not initialized");
                return;
            }

            // Error if length of file is greater than 4 with null included
            if (name.Length > 4)
            {
                Console.WriteLine("cannot accept names longer than 3 characters");
                return;
            }

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
                /*if (FSfile.SetFile(block, name, descriptor))
                {
                    _memcache.SetDescriptorLength(descriptor, 0);
                    return;
                }*/
            }

            // If a open file was not found and a new block can be added, add it.
            if (blocks.Count != 3)
            {
                var newblock = _memcache.GetOpenBlock();
                _memcache.SetBlockToDescriptor(descriptor, newblock);
                    
                //FSfile.SetFile(newblock, name, descriptor);
                _memcache.SetDescriptorLength(descriptor, 0);
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
            Console.WriteLine(name + " created");
                
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
        /// <param name="index">The index in the OFT.</param>
        /// <param name="count">The count of bytes to return back.</param>
        /// <returns></returns>
        public sbyte[] Read(int index, int count)
        {
            // Get OftFile from OFT
            var oftFile = _oft.GetFile(index);

            // Get Descriptor
            var fd = _memcache.GetFileDescriptorByIndex(oftFile.index);

            // Save current position
            var pos = oftFile.position;

            // Save current block index
            var blockIndex = oftFile.position/64;

            // Save total blocks initialized
            var maxBlocks = fd.map.Select(x => x).Count(y => y != -1);

            // Initialize byte array
            var bytes = new List<sbyte>();

            // loop through block until desired count or end of file reached or end of buffer is reached.
            for (var i = 0; i < count; i++)
            {
                // if exhausted the whole block
                if ((pos != 0) && (pos%MaxBlockLength == 0))
                {
                    // if exhausted whole file, return bytes
                    if (pos/MaxBlockLength == maxBlocks)
                    {
                        return bytes.ToArray();
                    }
                    // else write block back and read the next block into 
                    // the oft/its file and reposition
                    else
                    {
                        // write buffer to disk
                        _ldisk.SetBlock(oftFile.block, fd.map[blockIndex]);

                        blockIndex++;

                        // read the new block
                        oftFile.block = _ldisk.ReadBlock(fd.map[blockIndex]);
                        pos = 0;

                        // Write oftFile back to OFT
                        _oft.SetFilePosition(index, pos);
                    }
                }

                bytes.Add(oftFile.block.data[pos]);
                pos++;
            }

            return bytes.ToArray();
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
        public void Lseek(int index, int pos)
        {
            var oft = _oft.GetFile(index);

            if (oft != null)
            {
                var newBlock = pos/64;

                // if new position is still in current block
                if (oft.index == newBlock)
                {
                    _oft.SetFilePosition(index, pos);
                }
                else
                {
                    // Get descriptor from OFTfile
                    var descriptor = _memcache.GetFileDescriptorByIndex(oft.index);

                    // write buffer to disk
                    _ldisk.SetBlock(oft.block, descriptor.map[oft.index]);

                    // read the new block
                    oft.block = _ldisk.ReadBlock(descriptor.map[newBlock]);

                    // Set the current position to new position
                    oft.position = pos;

                    // Write oftFile back to OFT
                    _oft.SetFilePosition(index, pos);
                }
            }
            else
            {
                Console.WriteLine("Invalid file index");   
            }
        }

        /// <summary>
        /// Prints out the list of directories
        /// </summary>
        /// <returns></returns>
        public List<string> Directories()
        {
            return new List<string>();
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

                // initialize file descriptor for directory
                var freeBlock = _memcache.GetOpenBlock();
                var fd = _memcache.GetFileDescriptorByIndex(DirectoryFileDescriptor);
                fd = new FileDescriptor
                {
                    length = 0,
                    map = new [] {freeBlock, -1, -1}
                };
                _memcache.SetFileDescriptorByIndex(DirectoryFileDescriptor, fd);


                // Add directory to OFT
                _oft.AddFile(_ldisk.ReadBlock(freeBlock), 0, 0);

                Console.WriteLine("disk initialized");
            }
            else
            {
                /*
                // initialize directory descriptor
                var map = GetDescriptorMap(0);

                // If directory's map has not been set, set it. If it has, don't worry
                // because it should have been stored in bitmap.
                if (map.Count == 0)
                {
                    var freeBlock = GetOpenBlock();
                    _fileDescriptors[0] = new FileDescriptor(0, new[]
                {
                    freeBlock, -1, -1
                });

                    _bitmap.SetBit(freeBlock);
                }*/
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

        public bool SetFile(int block, string name, int descriptor)
        {
            var data = _ldisk.ReadBlock(block).data;
            var index = 0;

            // 55 == 63 - 8 (size of file block)
            while (index < 55)
            {
                var bytes = new[]
                    {
                        data[index],
                        data[index + 1],
                        data[index + 2],
                        data[index + 3]
                    };
                var value = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                if (value == -1)
                {
                    var desc = BitConverter.GetBytes(descriptor);

                    data[index] = (sbyte)name[0];
                    data[index + 1] = (sbyte)name[1];
                    data[index + 2] = (sbyte)name[2];
                    data[index + 3] = (sbyte)name[3];

                    data[index + 4] = (sbyte)desc[0];
                    data[index + 5] = (sbyte)desc[1];
                    data[index + 6] = (sbyte)desc[2];
                    data[index + 7] = (sbyte)desc[3];

                    //_ldisk.SetBlock(data);

                    return true;
                }


                index += 8;
            }

            return false;
        }
    }
}
