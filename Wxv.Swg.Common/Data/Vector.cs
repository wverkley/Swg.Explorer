using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    public class Vector
    {
        public const int Size = 3;

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static readonly Vector Zero = new Vector { X = 0, Y = 0, Z = 0 };

        public Vector() { }
        public Vector(double[] data)
        {
            if ((data == null) || (data.Length != Size))
                throw new ArgumentException();

            this.X = data[0];
            this.Y = data[1];
            this.Z = data[2];
        }

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
}
