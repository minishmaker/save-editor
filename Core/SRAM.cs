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

        public readonly byte[] romData;
        public readonly Reader reader;

        public RegionVersion version { get; private set; } = RegionVersion.None;


        public SRAM(string filePath)
        {
            Instance = this;
            path = filePath;
            romData = File.ReadAllBytes(filePath);
            Stream stream = Stream.Synchronized(new MemoryStream(romData));
            reader = new Reader(stream);
            Debug.WriteLine("Read " + stream.Length + " bytes.");

            // temp test checksum
            uint sum = ReadChecksum(0);
            uint calcsum = CalculateChecksum(0);

            if (sum == calcsum) Console.WriteLine("Checksum correct.");
        }

        public uint ReadChecksum(int fileID)
        {
            uint sum = (uint)(reader.ReadUInt16(0x30 + (fileID * 0x10)) << 16);
            sum += reader.ReadUInt16();

            return sum;
        }

        public uint CalculateChecksum(int fileID)
        {
            byte[] gameState = reader.ReadBytes(0x04, 0x34 + (fileID * 0x10));

            byte[] gameData = reader.ReadBytes(0x500, 0x80 + (fileID * 0x500));

            uint shortcheck = PartialCheck(gameState);
            uint longcheck = PartialCheck(gameData);

            return Combine(shortcheck, longcheck);
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

        private uint Combine(uint shortcheck, uint longcheck)
        {
            uint combined = (shortcheck + longcheck) & 0xFFFF;
            uint upper = combined << 16;
            uint lower = ~combined & 0xFFFF;

            lower += 1;
            combined = upper + lower;

            return combined;
        }
    }
}
