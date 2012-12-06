using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    public struct Vector
    {
        public Single X { get; set; }
        public Single Y { get; set; }
        public Single Z { get; set; }

        public override string ToString()
        {
            return string.Format("X:{0:F3},Y:{1:F3},Z:{2:F3}", X, Y, Z);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public static bool Equals(Vector a, Vector b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public override bool Equals(object obj)
        {
            return obj != null && Equals(this, (Vector)obj);
        }

        public static Vector Load(Stream stream)
        {
            return new Vector
            {
                X = stream.ReadSingle(),
                Y = stream.ReadSingle(),
                Z = stream.ReadSingle()
            };
        }
    }

    public struct ColorS
    {
        public Single A { get; set; }
        public Single R { get; set; }
        public Single G { get; set; }
        public Single B { get; set; }

        public override string ToString()
        {
            return string.Format("A:{0:F3},R:{1:F3},G:{2:F3},B:{3:F3}", A, R, G, B);
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode();
        }

        public static bool Equals(ColorS a, ColorS b)
        {
            return a.A == b.A && a.R == b.R && a.G == b.G && a.B == b.B;
        }

        public override bool Equals(object obj)
        {
            return obj != null && Equals(this, (ColorS)obj);
        }

        public static ColorS Load(Stream stream)
        {
            return new ColorS
            {
                A = (byte)stream.ReadSingle(),
                R = (byte)stream.ReadSingle(),
                G = (byte)stream.ReadSingle(),
                B = (byte)stream.ReadSingle()
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

    public struct TextureCoord
    {
        public Single U { get; set; }
        public Single V { get; set; }

        public override string ToString()
        {
            return string.Format("U:{0:F3},V:{1:F3}", U, V);
        }

        public override int GetHashCode()
        {
            return U.GetHashCode() ^ V.GetHashCode();
        }

        public static bool Equals(TextureCoord a, TextureCoord b)
        {
            return a.U == b.U && a.V == b.V;
        }

        public override bool Equals(object obj)
        {
            return obj != null && Equals(this, (TextureCoord)obj);
        }

        public static TextureCoord Load(Stream stream)
        {
            return new TextureCoord
            {
                U = stream.ReadSingle(),
                V = stream.ReadSingle()
            };
        }
    }

    public class Matrix
    {
        public const int SizeX = 3;
        public const int SizeY = 3;

        private Single[,] data = new float[SizeX, SizeY];

        public Single GetValue(int x, int y) { return data[x, y]; }
        public void SetValue(int x, int y, Single value) { data[x, y] = value; }
        public Single this[int x, int y]
        {
            get { return GetValue(x, y); }
            set { SetValue(x, y, value); }
        }

        public override string ToString()
        {
            return string.Format("{0:F3},{1:F3},{2:F3},{3:F3},{4:F3},{5:F3},{6:F3},{7:F3},{8:F3}", this[0, 0], this[1, 0], this[2, 0], this[0, 1], this[1, 1], this[2, 1], this[0, 2], this[1, 2], this[2, 2]);
        }

        public override int GetHashCode()
        {
            return 
                this[0, 0].GetHashCode() 
                ^ this[1, 0].GetHashCode() 
                ^ this[2, 0].GetHashCode() 
                ^ this[0, 1].GetHashCode() 
                ^ this[1, 1].GetHashCode() 
                ^ this[2, 1].GetHashCode() 
                ^ this[0, 2].GetHashCode() 
                ^ this[1, 2].GetHashCode() 
                ^ this[2, 2].GetHashCode();
        }

    }

}
