using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    public struct ColorF
    {
        public double A { get; set; }
        public double R { get; set; }
        public double G { get; set; }
        public double B { get; set; }

        public override string ToString()
        {
            return string.Format("A:{0:F3},R:{1:F3},G:{2:F3},B:{3:F3}", A, R, G, B);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        }

        public static bool Equals(ColorF a, ColorF b)
        {
            return a.A == b.A && a.R == b.R && a.G == b.G && a.B == b.B;
        }

        public override bool Equals(object obj)
        {
            return obj != null && Equals(this, (ColorF)obj);
        }

        public static ColorF Load(Stream stream)
        {
            return new ColorF
            {
                A = stream.ReadSingle(),
                R = stream.ReadSingle(),
                G = stream.ReadSingle(),
                B = stream.ReadSingle()
            };
        }
    }

    public struct Color
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public override string ToString()
        {
            return string.Format("A:{0},R:{1},G:{2},B:{3}", A, R, G, B);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        }

        public static bool Equals(Color a, Color b)
        {
            return a.A == b.A && a.R == b.R && a.G == b.G && a.B == b.B;
        }

        public override bool Equals(object obj)
        {
            return obj != null && Equals(this, (Color)obj);
        }

        public static Color Load(Stream stream)
        {
            return new Color
            {
                A = (byte) stream.ReadByte(),
                R = (byte) stream.ReadByte(),
                G = (byte) stream.ReadByte(),
                B = (byte) stream.ReadByte()
            };
        }
    }
}
