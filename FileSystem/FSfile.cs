using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    /// <summary>
    /// This is the data structure for a file.
    /// </summary>
    class FSfile
    {
        private char[] name;
        private int index;
        private Ldisk _ldisk = Ldisk.Instance;

        public static bool OpenFileExists(int block)
        {
            /*var index = 0;
            var b = block.data;
            while (index < 64)
            {
                var bytes = new[]
                    {
                        b[index],
                        b[index + 1],
                        b[index + 2],
                        b[index + 3]
                    };
                var value = BitConverter.ToInt32((byte[])(Array)bytes, 0);

                if (value == -1)
                {
                    /*
                    return new FSfile(
                    {
                        name = new char[4] { 'a', 'b', 'c', 'd'},
                        index = 4
                    };
                     *
                }
            }*/

            return false;
        }

        public static void SetFile(string s, int descriptor)
        {
            throw new NotImplementedException();
        }
    }
}
