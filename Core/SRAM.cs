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

            SetupRom();
        }

        private void SetupRom()
        {
            // Determine game region and if valid ROM
            byte[] regionBytes = reader.ReadBytes(4, 0xAC);
            string region = System.Text.Encoding.UTF8.GetString(regionBytes);
            Debug.WriteLine("Region detected: " + region);

            if (region == "BZMP")
            {
                version = RegionVersion.EU;
            }

            if (region == "BZMJ")
            {
                version = RegionVersion.JP;
            }

            if (region == "BZME")
            {
                version = RegionVersion.US;
            }
        }
    }
}
