using System;
using System.Diagnostics;
using System.IO;
using SaveEditor.Utilities;

namespace SaveEditor.Core
{
    public enum RegionVersion
    {
        EU,
        JP,
        US,
        None
    }

    public class SRAM
    {
        public static SRAM Instance { get; private set; }
        public readonly string path;

        private byte[] saveData;
        public readonly Reader reader;
        public readonly Writer writer;

        public RegionVersion version { get; private set; } = RegionVersion.None;


        public SRAM(string filePath)
        {
            Instance = this;
            path = filePath;
            saveData = File.ReadAllBytes(filePath);
            
            File.WriteAllBytes(path+".bak", saveData);

            // Data is stored in reversed blocked of 8; when read by GBA it assumes the correct order, so we need to change it.
            for (int block = 0; block < saveData.Length;)
            {
                Array.Reverse(saveData, block, 8);
                block += 8;
            }

            Stream stream = Stream.Synchronized(new MemoryStream(saveData));
            reader = new Reader(stream);
            writer = new Writer(stream);


            Debug.WriteLine("Read " + stream.Length + " bytes.");
        }

        public SaveFile GetSaveFile(int fileNumber)
        {
            if (fileNumber < 0 || fileNumber > 2) throw new IndexOutOfRangeException("File number must be either 0, 1, 2.");

            return new SaveFile(ReadChecksum(fileNumber), ReadData(fileNumber));

        }

        public void SaveFile(SaveFile save, int fileNumber)
        {
            uint checksum = CalculateChecksum(fileNumber);
            writer.WriteBytes(save.data, 0x80 + (fileNumber * 0x500));

            uint newchecksum = CalculateChecksum(fileNumber);

            Console.WriteLine("old checksum {0}", StringUtil.AsStringHex8((int)checksum));
            Console.WriteLine("new checksum {0}", StringUtil.AsStringHex8((int)newchecksum));

            writer.WriteUInt16((ushort)((newchecksum & 0xFFFF0000) >> 16),0x30 + (fileNumber * 0x10));
            writer.WriteUInt16((ushort)(newchecksum & 0xFFFF));

            writer.Flush();

            byte[] dataToWrite = new byte[saveData.Length];

            Array.Copy(saveData, dataToWrite, saveData.Length);
            

            // Flip data back for output write, but don't touch current data
            for (int block = 0; block < dataToWrite.Length;)
            {
                Array.Reverse(dataToWrite, block, 8);
                block += 8;
            }

            if (dataToWrite == saveData) Console.WriteLine("Same!");
            
            File.WriteAllBytes(path, dataToWrite);

        }

        private uint ReadChecksum(int fileNumber)
        {
            uint sum = (uint)(reader.ReadUInt16(0x30 + (fileNumber * 0x10)) << 16);

            sum += reader.ReadUInt16();

            return sum;
        }

        private byte[] ReadData(int fileNumber)
        {
            return reader.ReadBytes(0x500, 0x80 + (fileNumber * 0x500));
        }

        public uint CalculateChecksum(int fileNumber)
        {
            byte[] gameState = reader.ReadBytes(0x04, 0x34 + (fileNumber * 0x10));

            byte[] gameData = reader.ReadBytes(0x500, 0x80 + (fileNumber * 0x500));

            uint shortcheck = PartialCheck(gameState);
            uint longcheck = PartialCheck(gameData);

            uint combined = (shortcheck + longcheck) & 0xFFFF;
            uint upper = combined << 16;
            uint lower = ~combined & 0xFFFF;

            lower += 1;
            combined = upper + lower;

            return combined;
        }

        private uint PartialCheck(byte[] data)
        {
            uint pos = 0;
            uint remaining = (uint)data.Length;
            uint sum = 0;

            while (remaining > 0)
            {
                sum += (uint)(data[pos] | (data[pos + 1] << 8)) ^ remaining;
                pos += 2;
                remaining -= 2;
            }

            sum &= 0xFFFF;

            return sum;
        }
    }
}
