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
                throw new Exception("Disk not initialized");
            }

            // Error if length of file is greater than 4 with null included
            if (name.Length > 4)
            {
                throw new Exception("Cannot accept names longer than 3 characters");
            }

            // Get OpenFileTable entry for directory
            var oftFile = _oft.GetFile(0);

            // Get directory file descriptor
            var fd = _memcache.GetFileDescriptorByIndex(DirectoryFileDescriptor);

            // Test to see if the file already exists
            Lseek(0, 0);

            // Search until max directory size
            while (oftFile.position != fd.length)
            {
                var oldPos = oftFile.position;

                // Get name and descriptor
                var n = Encoding.UTF8.GetString((byte[])(Array)Read(DirectoryFileDescriptor, 8), 0, 4).Trim(); // remove the padding when allocating

                if (n == name)
                {
                    throw new Exception("File already exists");
                }
            }

            // Pad file name with spaces
            name = name.PadRight(4, ' ');

            // Seek back to the beginning
            Lseek(DirectoryFileDescriptor, 0);

            // Find open file descriptor
            var descriptor = _memcache.GetOpenFileDescriptor();

            if (descriptor == -1)
            {
                throw new Exception("No empty file descriptors");
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

                    // allocate 1 block to descriptor
                    var freeBlock = _memcache.GetOpenBlock();
                    _memcache.SetBlockToDescriptor(descriptor, freeBlock);
                    return;
                }

                // Test if file is -1, if so add it.
                if (BitConverter.ToInt32((byte[]) (Array) bytes, 4) == -1)
                {
                    var characters = new List<char>();
                    characters.AddRange(name);
                    characters.AddRange(BitConverter.GetBytes(descriptor).Select(x => (char)x));

                    // update position
                    Lseek(DirectoryFileDescriptor, oldPos);

                    // write characters to buffer
                    foreach (var character in characters)
                    {
                        Write(0, character, 1);
                    }

                    _memcache.SetDescriptorLength(descriptor, 0);

                    // allocate 1 block to descriptor
                    var freeBlock = _memcache.GetOpenBlock();
                    _memcache.SetBlockToDescriptor(descriptor, freeBlock);
                    return;
                }
                

            }

            throw new Exception("Directory is full");
        }

        /// <summary>
        /// Destroys the specified file.
        /// </summary>
        /// <param name="name">The name.</param>
        public void Destroy(string deleteName)
        {
            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                throw new Exception("Disk not initialized");
            }

            // Error if length of file is greater than 4 with null included
            if (deleteName.Length > 4)
            {
                throw new Exception("Cannot accept names longer than 3 characters");
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
                    Lseek(0, oldPos);

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

                    return;
                }
            }

            throw new Exception("File not found");
        }

        /// <summary>
        /// Opens the specified file.
        /// </summary>
        /// <param name="openName">The name of the file to be opened.</param>
        /// <returns></returns>
        public int Open(string openName)
        {
            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                throw new Exception("Disk not initialized");
            }

            // Error if length of file is greater than 4 with null included
            if (openName.Length > 4)
            {
                throw new Exception("Cannot accept names longer than 3 characters");
            }

            // Seek back to the beginning
            Lseek(DirectoryFileDescriptor, 0);

            // Get OpenFileTable entry for directory
            var oftFile = _oft.GetFile(0);

            while (oftFile.position != MaxBlockLength * 3)
            {
                // Get name and descriptor
                var name = Encoding.UTF8.GetString((byte[])(Array)Read(DirectoryFileDescriptor, 4), 0, 4).Trim(); // remove the padding when allocating
                var descriptor = BitConverter.ToInt32((byte[])(Array)Read(DirectoryFileDescriptor, 4), 0);

                if (openName == name)
                {
                    var fd = _memcache.GetFileDescriptorByIndex(descriptor);

                    // check and see if file is already opened
                    for (int i = 0; i < 4; i++)
                    {
                        var f = _oft.GetFile(i);
                        if (f != null && f.index == descriptor)
                        {
                            throw new Exception("File is already opened");
                        }
                    }

                    var oftPos = _oft.GetFreeSlot();
                    
                    // insert into oft table
                    if (oftPos != -1)
                    {
                        _oft.OpenFile(oftPos, _ldisk.ReadBlock(fd.map[0]), 0, descriptor);
                        return oftPos;
                    }
                    else
                    {
                        throw new Exception("Too many open files");
                    }
                }
            }

            // file was not found
            throw new Exception("File not found");
        }

        /// <summary>
        /// Closes the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        public int Close(int file)
        {
            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                throw new Exception("Disk not initialized");
            }

            var oftFile = _oft.GetFile(file);

            // Error if unable to access OFT index
            if (oftFile == null || oftFile.index == 0)
            {
                throw new Exception("Invalid OFT index");
            }

            var fd = _memcache.GetFileDescriptorByIndex(oftFile.index);

            // write buffer to disk
            _ldisk.SetBlock(oftFile.block, fd.map[oftFile.position / 64]);

            // update file length in descriptor
            fd.length = 0;


            // free OFT entry
            _oft.CloseFile(file);

            return file;
        }

        /// <summary>
        /// Reads count numbers from the specified file at location index.
        /// </summary>
        /// <param name="index">The index in the OFT.</param>
        /// <param name="count">The count of bytes to return back.</param>
        /// <returns></returns>
        public sbyte[] Read(int index, int count)
        {
            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                throw new Exception("Disk not initialized");
            }

            // Get OftFile from OFT
            var oftFile = _oft.GetFile(index);

            // Error if length of file is greater than 4 with null included
            if (oftFile == null)
            {
                throw new Exception("Invalid OFT index");
            }

            // Get Descriptor
            var fd = _memcache.GetFileDescriptorByIndex(oftFile.index);

            // Save current block index
            var blockIndex = oftFile.position/64;

            // Initialize byte array
            var bytes = new List<sbyte>();

            // loop through block until desired count or end of file reached or end of buffer is reached.
            for (var i = 0; i < count; i++)
            {
                if (oftFile.position == fd.length)
                {
                    return bytes.ToArray();
                }

                bytes.Add(oftFile.block.data[oftFile.position % 64]);
                oftFile.position++;

                // if exhausted the whole block
                if (oftFile.position % MaxBlockLength == 0)
                {
                    // write buffer to disk
                    _ldisk.SetBlock(oftFile.block, fd.map[blockIndex]);

                    blockIndex++;

                    // read the new block
                    oftFile.block = _ldisk.ReadBlock(fd.map[blockIndex]);
                }

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
            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                throw new Exception("Disk not initialized");
            }

            // Get OftFile from OFT
            var oftFile = _oft.GetFile(index);

            // Error if length of file is greater than 4 with null included
            if (oftFile == null)
            {
                throw new Exception("Invalid OFT index");
            }

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
                bytesWritten++;

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
            }

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
            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                throw new Exception("Disk not initialized");
            }

            // Get OftFile from OFT
            var oft = _oft.GetFile(index);

            // Error if length of file is greater than 4 with null included
            if (oft == null)
            {
                throw new Exception("Invalid OFT index");
            }

            // Get descriptor from OFTfile
            var descriptor = _memcache.GetFileDescriptorByIndex(oft.index);

            if (descriptor.length < pos)
            {
                throw new Exception("Invalid seek position");
            }

            var newBlock = pos/64;
            var oldBlock = oft.position/64;

            // if new position is still in current block
            if (oldBlock == newBlock)
            {
                oft.position = pos;
            }
            else
            {
                // write buffer to disk
                _ldisk.SetBlock(oft.block, descriptor.map[oldBlock]);

                // read the new block
                oft.block = _ldisk.ReadBlock(descriptor.map[newBlock]);

                // Set the current position to new position
                oft.position = pos;

            }
            return true;
        }

        /// <summary>
        /// Prints out the list of directories
        /// </summary>
        /// <returns></returns>
        public List<string> Directories()
        {
            var directories = new List<string>();

            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                throw new Exception("Disk not initialized");
            }

            // Seek back to the beginning
            Lseek(DirectoryFileDescriptor, 0);

            // Get OpenFileTable entry for directory
            var oftFile = _oft.GetFile(0);

            var fd = _memcache.GetFileDescriptorByIndex(oftFile.index);

            while (oftFile.position < fd.length)
            {
                // Get name and descriptor
                var bytes = Read(DirectoryFileDescriptor, 8);

                if (BitConverter.ToInt32((byte[]) (Array) bytes, 4) != -1)
                {
                    var name = Encoding.UTF8.GetString((byte[])(Array)bytes, 0, 4).Replace('\0', ' ').Trim(); // remove the padding when allocating
                    directories.Add(name);
                }
            }

            return directories;
        }

        /// <summary>
        /// If filename is null, create a new file system. If the specified
        /// file exists, initialize the file system using that file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public bool Init(string filename)
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
                var oftPos = _oft.GetFreeSlot();
                _oft.OpenFile(oftPos, _ldisk.ReadBlock(freeBlock), 0, 0);

                return true;
            }
            else
            {
                var fileStream = new FileStream(filename, FileMode.Open);
                var blockArray = new sbyte[64][];
                try
                {                   
                    for (var i = 0; i < 64; i++)
                    {
                        var bytes = new sbyte[64];
                        fileStream.Read((byte[])(Array)bytes, 0, 64);

                        blockArray[i] = bytes;
                    }
                }
                finally
                {
                    _ldisk = new Ldisk(blockArray);
                    fileStream.Close();
                }

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

                // get directory file descriptor
                var fd = _memcache.GetFileDescriptorByIndex(DirectoryFileDescriptor);

                // open directory
                _oft.OpenFile(0, _ldisk.ReadBlock(fd.map[0]), 0, DirectoryFileDescriptor);

                return true;
            }
        }

        /// <summary>
        /// Saves the current state of the file system to the file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public void Save(string filename)
        {
            // If disk and cache have not been initialized error
            if ((_ldisk == null) || (_memcache == null))
            {
                throw new Exception("Disk not initialized");
            }

            // save all OFT blocks to disk
            for (int i = 0; i < 4; i++)
            {
                var oftFile = _oft.GetFile(i);

                if (oftFile != null)
                {
                    var fd = _memcache.GetFileDescriptorByIndex(oftFile.index);

                    _ldisk.SetBlock(oftFile.block, fd.map[oftFile.position/64]); // push the current block back to disk
                }
            }

            // save the bitmap back to disk
            _ldisk.SetBlock(_memcache.GetBitMap(), 0);

            // save the file descriptors to disk
            for (int i = 0; i < 6; i++)
            {
                var bytes = new List<sbyte>();
                for (int j = 0; j < 4; j++)
                {
                    var fd = _memcache.GetFileDescriptorByIndex((i*4) + j);
                    
                    // add length to bytes
                    bytes.AddRange(BitConverter.GetBytes(fd.length).Select(x => (sbyte)x));

                    // add map to bytes
                    foreach (var m in fd.map)
                    {
                        bytes.AddRange(BitConverter.GetBytes(m).Select(x => (sbyte)x));
                    }
                }

                _ldisk.SetBlock(new Block(bytes.ToArray()), i + 1);
            }

            // create file and override if exists
            var fileStream = new FileStream(filename, FileMode.Create);
            try
            {
                for (var i = 0; i < 64; i++)
                {
                    var block = _ldisk.ReadBlock(i);
                    fileStream.Write((byte[])(Array)block.data, 0, block.data.Length);
                }
            }
            finally
            {
                fileStream.Close();
            }
        }
    }
}
