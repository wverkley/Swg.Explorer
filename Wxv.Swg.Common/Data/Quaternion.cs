using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    public class Quaternion
    {
        public const int Size = 4;

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double W { get; set; }

        public double Length
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z + W * W); }
        }

        public double Length2
        {
            get { return X * X + Y * Y + Z * Z + W * W; }
        }

        public Quaternion() { }
        public Quaternion(double[] data)
        {
            if ((data == null) || (data.Length != Size))
                throw new ArgumentException();

            this.X = data[0];
            this.Y = data[1];
            this.Z = data[2];
            this.W = data[3];
        }

        public override string ToString()
        {
            return string.Format("X:{0:F3},Y:{1:F3},Z:{2:F3},W:{2:F3}", X, Y, Z, W);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public static bool Equals(Quaternion a, Quaternion b)
        {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
        }

        public override bool Equals(object obj)
        {
            return obj != null && Equals(this, (Vector)obj);
        }

        public static Quaternion Load(Stream stream)
        {
            return new Quaternion
            {
                X = stream.ReadSingle(),
                Y = stream.ReadSingle(),
                Z = stream.ReadSingle(),
                W = stream.ReadSingle(),
            };
        }

        public static Quaternion Multiply(Quaternion a, Quaternion b)
        {
            return new Quaternion
            {
                X = a.X * b.X  -  a.Y * b.Y  -  a.Z * b.Z  -  a.W * b.W,
                Y = a.X * b.Y  +  a.Y * b.X  +  a.Z * b.W  -  a.W * b.Z,
                Z = a.X * b.Z  -  a.Y * b.W  +  a.Z * b.X  +  a.W * b.Y,
                W = a.X * b.W  +  a.Y * b.Z  -  a.Z * b.Y  +  a.W * b.X,
            };
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            return Quaternion.Multiply(a, b);
        }

    }
}
