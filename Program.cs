using System;
using System.IO;

namespace r
{
    class Program
    {
        static void Main(string[] args)
        {
            for(int i = 0; i < args.Length; i++)
            {
                Replay r;

                try
                {
                    r = ReadReplay(args[i]);
                } catch { continue; }
                File.WriteAllBytes("Raw-" + args[i], r.ReplayData);
                Console.WriteLine($"Processed Replay: {args[i]} ({i + 1}/{args.Length})");
            }
        }

        static Replay ReadReplay(string path)
        {
            Replay r = new Replay();

            using (Reader ms = new Reader(new MemoryStream(File.ReadAllBytes(path))))
            {
                r.Gamemode = ms.ReadByte();
                r.OsuVersion = ms.ReadInt32();
                r.MapHash = ms.ReadString();
                r.Player = ms.ReadString();
                r.ReplayHash = ms.ReadString();
                r.Count300 = ms.ReadUInt16();
                r.Count100 = ms.ReadUInt16();
                r.Count50 = ms.ReadUInt16();
                r.CountGeki = ms.ReadUInt16();
                r.CountKatu = ms.ReadUInt16();
                r.CountMiss = ms.ReadUInt16();
                r.Score = ms.ReadInt32();
                r.Combo = ms.ReadUInt16();
                r.FullCombo = (ms.ReadByte() == 0x01);
                r.Mods = ms.ReadInt32();

                r.Lifebar = ms.ReadString();
                r.TimeTicks = ms.ReadInt64();
                r.ReplayLength = ms.ReadInt32();
                r.ReplayData = ms.ReadBytes((int)(ms.BaseStream.Length - ms.BaseStream.Position - 8));
                r.ReplayId = ms.ReadInt64();
            }

            return r;
        }
    }

    class Reader : BinaryReader
    {
        public Reader(Stream input) : base(input) { }
        public override string ReadString() { return base.ReadByte() == 0x0b ? base.ReadString() : ""; }
    }

    struct Replay
    {
        public byte Gamemode;
        public int OsuVersion;
        public string MapHash, Player, ReplayHash;
        public ushort Count300, Count100, Count50, CountGeki, CountKatu, CountMiss, Combo;
        public int Score, Mods;
        public bool FullCombo;
        
        public string Lifebar;
        public long TimeTicks;
        public int ReplayLength;
        public byte[] ReplayData;
        public long ReplayId;
    }
}
