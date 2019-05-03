using System;
using System.CodeDom;
using System.IO;
using System.Text;
using SaveEditor.Utilities;

namespace SaveEditor.Core
{
    public class SaveFile
    {
        public uint checksum { get; private set; }

        private byte[] data;
        private Reader reader_;

        public SaveFile(uint fileChecksum, byte[] fileData)
        {
            checksum = fileChecksum;
            data = fileData;

            Stream stream = Stream.Synchronized(new MemoryStream(data));
            reader_ = new Reader(stream);

            Console.WriteLine(GetName());
        }

        public string GetName()
        {
            return Encoding.Default.GetString(reader_.ReadBytes(6, 0x80));
        }
    }
}