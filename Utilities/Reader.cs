using System.IO;

namespace SaveEditor.Utilities
{
    public class Reader
    {
        private readonly Stream stream_;

        private readonly BinaryReader reader_;

        public Reader(Stream stream)
        {
            stream_ = stream;
            reader_ = new BinaryReader(stream);
        }

        public long Position => stream_.Position;

        public void SetPosition(long pos)
        {
            stream_.Position = pos;
        }

        public byte PeekByte()
        {
            long tempPos = stream_.Position;
            byte b = ReadByte();
            stream_.Position = tempPos;
            return b;
        }

        public byte PeekByte(long pos)
        {
            stream_.Position = pos;
            return PeekByte();
        }

        public byte[] PeekBytes(int num)
        {
            long tempPos = stream_.Position;
            byte[] b = ReadBytes(num);
            stream_.Position = tempPos;
            return b;
        }

        public byte[] PeekBytes(int num, long pos)
        {
            stream_.Position = pos;
            return PeekBytes(num);
        }

        public byte ReadByte()
        {
            return reader_.ReadByte();
        }

        public byte ReadByte(long pos)
        {
            stream_.Position = pos;
            return ReadByte();
        }

        public byte[] ReadBytes(int num)
        {
            return reader_.ReadBytes(num);
        }

        public byte[] ReadBytes(int num, long pos)
        {
            stream_.Position = pos;
            return ReadBytes(num);
        }

        public ushort ReadUInt16()
        {
            return reader_.ReadUInt16();
        }

        public ushort ReadUInt16(long pos)
        {
            stream_.Position = pos;
            return reader_.ReadUInt16();
        }

        public short ReadInt16()
        {
            return reader_.ReadInt16();
        }

        public short ReadInt16(long pos)
        {
            stream_.Position = pos;
            return reader_.ReadInt16();
        }

        public uint ReadUInt32()
        {
            return reader_.ReadUInt32();
        }

        public uint ReadUInt32(long pos)
        {
            stream_.Position = pos;
            return reader_.ReadUInt32();
        }

        public int ReadInt()
        {
            return reader_.ReadInt32();
        }

        public int ReadInt(long pos)
        {
            stream_.Position = pos;
            return reader_.ReadInt32(); ;
        }

        public int ReadAddr()
        {
            return ReadInt() & 0xFFFFFF;
        }

        public int ReadAddr(long pos)
        {
            stream_.Position = pos;
            return ReadAddr();
        }
    }
}
