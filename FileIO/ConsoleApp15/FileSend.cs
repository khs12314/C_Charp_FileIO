using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIO
{
    interface IFileSend
    {
        void Initialize();
        void Send(string filePath);
        void Send(byte[] data);
        void Close();
    }
    internal class FileSend : IFileSend
    {

    }
}
