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
        public bool Create(string name)
        {
            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                Console.WriteLine("Disk not initialized");
                return false;
            }

            // Error if length of file is greater than 4 with null included
            if (name.Length > 4)
            {
                Console.WriteLine("cannot accept names longer than 3 characters");
                return false;
            }

            name = name.PadRight(4, ' ');
            
            // Seek back to the beginning
            Lseek(DirectoryFileDescriptor, 0);

            // Get OpenFileTable entry for directory
            var oftFile = _oft.GetFile(0);

            // Find open file descriptor
            var descriptor = _memcache.GetOpenFileDescriptor();

            if (descriptor == -1)
            {
                Console.WriteLine("No empty file descriptors");
                return false;
            }


            // Search until max directory size
            while (oftFile.position != MaxBlockLength * 3)
            {
                var bytes = new sbyte[8];
                var oldPos = oftFile.position;
                bytes = Read(DirectoryFileDescriptor, 8);

                // Reached end of file, but there is room to append
                if (bytes.Count() == 0)
                {
                    var characters = new List<char>();
                    characters.AddRange(name);
                    characters.AddRange(BitConverter.GetBytes(descriptor).Select(x => (char)x));

                    // write characters to buffer
                    foreach (var character in characters)
                    {
                        Write(0, character, 1);
                    }

                    _memcache.SetDescriptorLength(descriptor, 0);
                    return true;
                }

                // Test if file is -1, if so add it.
                if (BitConverter.ToInt32((byte[]) (Array) bytes, 4) == -1)
                {
                    //var df = new DirectoryFile(name.ToCharArray(), descriptor);
                    //public int Write(int index, char character, int count)
                    var characters = new List<char>();
                    characters.AddRange(name);
                    characters.AddRange(BitConverter.GetBytes(descriptor).Select(x => (char)x));

                    // update position
                    oftFile.position = oldPos;
                    _oft.UpdateFile(0, oftFile);

                    // write characters to buffer
                    foreach (var character in characters)
                    {
                        Write(0, character, 1);
                    }

                    _memcache.SetDescriptorLength(descriptor, 0);
                    return true;
                }
                

            }

            Console.WriteLine("Directory is full");
            return false;
        }

        /// <summary>
        /// Destroys the specified file.
        /// </summary>
        /// <param name="name">The name.</param>
        public bool Destroy(string deleteName)
        {
            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                Console.WriteLine("Disk not initialized");
                return false;
            }

            // Error if length of file is greater than 4 with null included
            if (deleteName.Length > 4)
            {
                Console.WriteLine("cannot accept names longer than 3 characters");
                return false;
            }

            // Seek back to the beginning
            Lseek(DirectoryFileDescriptor, 0);

            // Get OpenFileTable entry for directory
            var oftFile = _oft.GetFile(0);

            while (oftFile.position != MaxBlockLength*3)
            {
                var oldPos = oftFile.position;

                // Get name and descriptor
                var name = Encoding.UTF8.GetString((byte[])(Array)Read(DirectoryFileDescriptor, 4), 0, 4).Trim(); // remove the padding when allocating
                var descriptor = BitConverter.ToInt32((byte[])(Array)Read(DirectoryFileDescriptor, 4), 0);

                if (deleteName == name)
                {
                    var fd = _memcache.GetFileDescriptorByIndex(descriptor);
                    oftFile.position = oldPos;

                    sbyte emptySbyte = -1;

                    // Remove directory entry
                    Write(0, (char)emptySbyte, 8);

                    var blocks = fd.map.Where(x => x != -1).Select(y => y);


                    // update bitmap
                    foreach (var block in blocks)
                    {
                        _memcache.ClearBlock(block);
                    }

                    // free file descriptor
                    fd.length = -1;
                    fd.map = new[] {-1, -1, -1};

                    return true;
                }
            }

            Console.WriteLine("File not found");
            return false;
        }

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

            // Save current block index
            var blockIndex = oftFile.position/64;

            // Save total blocks initialized
            //var maxBlocks = fd.map.Select(x => x).Count(y => y != -1);

            // Initialize byte array
            var bytes = new List<sbyte>();

            // loop through block until desired count or end of file reached or end of buffer is reached.
            for (var i = 0; i < count; i++)
            {
                if (oftFile.position == fd.length)
                {
                    _oft.UpdateFile(index, oftFile);
                    return bytes.ToArray();
                }

                bytes.Add(oftFile.block.data[oftFile.position % 64]);
                oftFile.position++;

                // if exhausted the whole block
                if (oftFile.position % MaxBlockLength == 0)
                {
                    // if exhausted whole file, return bytes
                    /*if (oftFile.position/MaxBlockLength == maxBlocks)
                    {
                        _oft.UpdateFile(index, oftFile);
                        return bytes.ToArray();
                     * 
                     * THIS MIGHT BE UNNECESSARY NOW
                    }*/
                    // else write block back and read the next block into 
                    // the oft/its file and reposition
                    //else
                    //{
                    // write buffer to disk
                    _ldisk.SetBlock(oftFile.block, fd.map[blockIndex]);

                    blockIndex++;

                    // read the new block
                    oftFile.block = _ldisk.ReadBlock(fd.map[blockIndex]);

                    // Write oftFile back to OFT
                    _oft.UpdateFile(index, oftFile);
                    //}
                }

            }

            _oft.UpdateFile(index, oftFile);
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
            // Get OftFile from OFT
            var oftFile = _oft.GetFile(index);

            // Get Descriptor
            var fd = _memcache.GetFileDescriptorByIndex(oftFile.index);

            // Save current block index
            var blockIndex = oftFile.position / 64;

            var bytesWritten = 0;

            // loop through block until desired count or end of file reached or end of buffer is reached.
            for (var i = 0; i < count; i++)
            {
                oftFile.block.data[oftFile.position % MaxBlockLength] = (sbyte)character;
                oftFile.position++;

                // if exhausted the whole block
                if (oftFile.position % MaxBlockLength == 0)
                {
                    // Reached max file length
                    if (oftFile.position / 64 == 3)
                    {
                        fd.length += bytesWritten;
                        return bytesWritten;
                    }

                    // If next block already exists
                    if (fd.map[oftFile.position / 64] != -1)
                    {
                        // write buffer to disk
                        _ldisk.SetBlock(oftFile.block, fd.map[blockIndex]);

                        blockIndex++;

                        // read the new block
                        oftFile.block = _ldisk.ReadBlock(fd.map[blockIndex]);
                        oftFile.position = 0;

                        // Write oftFile back to OFT
                        _oft.UpdateFile(index, oftFile);
                    }
                    else
                    {
                        // write buffer to disk
                        _ldisk.SetBlock(oftFile.block, fd.map[blockIndex]);

                        blockIndex++;

                        var newBlock = _memcache.GetOpenBlock();
                        _memcache.SetBlockToDescriptor(index, newBlock);
                        oftFile.block = _ldisk.ReadBlock(fd.map[blockIndex]);
                    }

                }

                // if at new block posiiton, read in the next block
                /*if (oftFile.position % MaxBlockLength == 0 && oftFile.position / 64 != 3)
                {
                    _ldisk.SetBlock(oftFile.block, fd.map[blockIndex]);

                    blockIndex++;

                    var newBlock = _memcache.GetOpenBlock();
                    _memcache.SetBlockToDescriptor(index, newBlock);
                    oftFile.block = _ldisk.ReadBlock(fd.map[blockIndex]);
                    _oft.UpdateFile(index, oftFile);
                }*/

                bytesWritten++;
            }

            _oft.UpdateFile(index, oftFile);
            fd.length += bytesWritten;
            return bytesWritten;
        }

        /// <summary>
        /// Seeks to the specified position in the file.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="pos">The position.</param>
        public bool Lseek(int index, int pos)
        {
            var oft = _oft.GetFile(index);

            if (oft != null)
            {
                var newBlock = pos/64;
                var oldBlock = oft.position/64;

                // if new position is still in current block
                if (oldBlock == newBlock)
                {
                    oft.position = pos;
                    _oft.UpdateFile(0, oft);
                }
                else
                {
                    // Get descriptor from OFTfile
                    var descriptor = _memcache.GetFileDescriptorByIndex(oft.index);

                    // write buffer to disk
                    _ldisk.SetBlock(oft.block, descriptor.map[oldBlock]);

                    // read the new block
                    oft.block = _ldisk.ReadBlock(descriptor.map[newBlock]);

                    // Set the current position to new position
                    oft.position = pos;

                    // Write oftFile back to OFT
                    _oft.UpdateFile(0, oft);

                }
                return true;
            }

            Console.WriteLine("Invalid file index");
            return false;
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

                // Set freeblock in bitmap for directory
                _memcache.SetBlock(freeBlock);

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
    }
}
