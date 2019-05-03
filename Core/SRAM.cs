using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
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

        private byte[] romData;
        public readonly Reader reader;

        public RegionVersion version { get; private set; } = RegionVersion.None;


        public SRAM(string filePath)
        {
            Instance = this;
            path = filePath;
            romData = File.ReadAllBytes(filePath);

            // Data is stored in reversed blocked of 8; when read by GBA it assumes the correct order, so we need to change it.
            for (int block = 0; block < romData.Length;)
            {
                Array.Reverse(romData, block, 8);
                block += 8;
            }

            Stream stream = Stream.Synchronized(new MemoryStream(romData));
            reader = new Reader(stream);
            Debug.WriteLine("Read " + stream.Length + " bytes.");

            // temp test checksum
            uint sum = ReadChecksum(0);
            uint calcsum = CalculateChecksum(0);

            if (sum == calcsum) Console.WriteLine("Checksum correct.");
        }

        public SaveFile GetSaveFile(int fileNumber)
        {

            return new SaveFile(ReadChecksum(fileNumber), ReadData(fileNumber));

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
