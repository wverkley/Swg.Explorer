using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Wxv.Swg.Common
{
    public class Matrix
    {
        public static readonly Matrix Identity = new Matrix
        {
            v_0_0 = 1,
            v_1_0 = 0,
            v_2_0 = 0,
            v_3_0 = 0,
            v_0_1 = 0,
            v_1_1 = 1,
            v_2_1 = 0,
            v_3_1 = 0,
            v_0_2 = 0,
            v_1_2 = 0,
            v_2_2 = 1,
            v_3_2 = 0,
            v_0_3 = 0,
            v_1_3 = 0,
            v_2_3 = 0,
            v_3_3 = 1
        };

        public const int SizeX = 4;
        public const int SizeY = 4;

        public double v_0_0;
        public double v_1_0;
        public double v_2_0;
        public double v_3_0;
        public double v_0_1;
        public double v_1_1;
        public double v_2_1;
        public double v_3_1;
        public double v_0_2;
        public double v_1_2;
        public double v_2_2;
        public double v_3_2;
        public double v_0_3;
        public double v_1_3;
        public double v_2_3;
        public double v_3_3;

        public double GetValue(int x, int y)
        {
            if (x < 0 || x >= SizeX) throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= SizeY) throw new ArgumentOutOfRangeException("y");

            switch (y)
            {
                case 0: switch (x)
                    {
                        case 0: return v_0_0;
                        case 1: return v_1_0;
                        case 2: return v_2_0;
                        default: return v_3_0;
                    }
                case 1: switch (x)
                    {
                        case 0: return v_0_1;
                        case 1: return v_1_1;
                        case 2: return v_2_1;
                        default: return v_3_1;
                    }
                case 2: switch (x)
                    {
                        case 0: return v_0_2;
                        case 1: return v_1_2;
                        case 2: return v_2_2;
                        default: return v_3_2;
                    }
                default: switch (x)
                    {
                        case 0: return v_0_3;
                        case 1: return v_1_3;
                        case 2: return v_2_3;
                        default: return v_3_3;
                    }
            }
        }
        public void SetValue(int x, int y, double value)
        {
            if (x < 0 || x >= SizeX) throw new ArgumentOutOfRangeException("x");
            if (y < 0 || y >= SizeY) throw new ArgumentOutOfRangeException("y");

            switch (y)
            {
                case 0: switch (x)
                    {
                        case 0: v_0_0 = value; return;
                        case 1: v_1_0 = value; return;
                        case 2: v_2_0 = value; return;
                        default: v_3_0 = value; return;
                    }
                case 1: switch (x)
                    {
                        case 0: v_0_1 = value; return;
                        case 1: v_1_1 = value; return;
                        case 2: v_2_1 = value; return;
                        default: v_3_1 = value; return;
                    }
                case 2: switch (x)
                    {
                        case 0: v_0_2 = value; return;
                        case 1: v_1_2 = value; return;
                        case 2: v_2_2 = value; return;
                        default: v_3_2 = value; return;
                    }
                default: switch (x)
                    {
                        case 0: v_0_3 = value; return;
                        case 1: v_1_3 = value; return;
                        case 2: v_2_3 = value; return;
                        default: v_3_3 = value; return;
                    }
            }
        }
        public double this[int x, int y]
        {
            get { return GetValue(x, y); }
            set { SetValue(x, y, value); }
        }

        internal Matrix() { }

        public override string ToString()
        {
            return string.Format(@"
{0:F3},{1:F3},{2:F3},{3:F3}, 
{4:F3},{5:F3},{6:F3},{7:F3}, 
{8:F3},{9:F3},{10:F3},{11:F3}, 
{12:F3},{13:F3},{14:F3},{15:F3}",
                v_0_0, v_1_0, v_2_0, v_3_0,
                v_0_1, v_1_1, v_2_1, v_3_1,
                v_0_2, v_1_2, v_2_2, v_3_2,
                v_0_3, v_1_3, v_2_3, v_3_3);
        }

        public override int GetHashCode()
        {
            return
                  v_0_0.GetHashCode()
                ^ v_1_0.GetHashCode()
                ^ v_2_0.GetHashCode()
                ^ v_3_0.GetHashCode()
                ^ v_0_1.GetHashCode()
                ^ v_1_1.GetHashCode()
                ^ v_2_1.GetHashCode()
                ^ v_3_1.GetHashCode()
                ^ v_0_2.GetHashCode()
                ^ v_1_2.GetHashCode()
                ^ v_2_2.GetHashCode()
                ^ v_3_2.GetHashCode()
                ^ v_0_3.GetHashCode()
                ^ v_1_3.GetHashCode()
                ^ v_2_3.GetHashCode()
                ^ v_3_3.GetHashCode();
        }

        public static Matrix Multiply(Matrix a, Matrix b)
        {
            return new Matrix
            {
                v_0_0 = a.v_0_0 * b.v_0_0 + a.v_1_0 * b.v_0_1 + a.v_2_0 * b.v_0_2 + a.v_3_0 * b.v_0_3,
                v_0_1 = a.v_0_1 * b.v_0_0 + a.v_1_1 * b.v_0_1 + a.v_2_1 * b.v_0_2 + a.v_3_1 * b.v_0_3,
                v_0_2 = a.v_0_2 * b.v_0_0 + a.v_1_2 * b.v_0_1 + a.v_2_2 * b.v_0_2 + a.v_3_2 * b.v_0_3,
                v_0_3 = a.v_0_3 * b.v_0_0 + a.v_1_3 * b.v_0_1 + a.v_2_3 * b.v_0_2 + a.v_3_3 * b.v_0_3,
                v_1_0 = a.v_0_0 * b.v_1_0 + a.v_1_0 * b.v_1_1 + a.v_2_0 * b.v_1_2 + a.v_3_0 * b.v_1_3,
                v_1_1 = a.v_0_1 * b.v_1_0 + a.v_1_1 * b.v_1_1 + a.v_2_1 * b.v_1_2 + a.v_3_1 * b.v_1_3,
                v_1_2 = a.v_0_2 * b.v_1_0 + a.v_1_2 * b.v_1_1 + a.v_2_2 * b.v_1_2 + a.v_3_2 * b.v_1_3,
                v_1_3 = a.v_0_3 * b.v_1_0 + a.v_1_3 * b.v_1_1 + a.v_2_3 * b.v_1_2 + a.v_3_3 * b.v_1_3,
                v_2_0 = a.v_0_0 * b.v_2_0 + a.v_1_0 * b.v_2_1 + a.v_2_0 * b.v_2_2 + a.v_3_0 * b.v_2_3,
                v_2_1 = a.v_0_1 * b.v_2_0 + a.v_1_1 * b.v_2_1 + a.v_2_1 * b.v_2_2 + a.v_3_1 * b.v_2_3,
                v_2_2 = a.v_0_2 * b.v_2_0 + a.v_1_2 * b.v_2_1 + a.v_2_2 * b.v_2_2 + a.v_3_2 * b.v_2_3,
                v_2_3 = a.v_0_3 * b.v_2_0 + a.v_1_3 * b.v_2_1 + a.v_2_3 * b.v_2_2 + a.v_3_3 * b.v_2_3,
                v_3_0 = a.v_0_0 * b.v_3_0 + a.v_1_0 * b.v_3_1 + a.v_2_0 * b.v_3_2 + a.v_3_0 * b.v_3_3,
                v_3_1 = a.v_0_1 * b.v_3_0 + a.v_1_1 * b.v_3_1 + a.v_2_1 * b.v_3_2 + a.v_3_1 * b.v_3_3,
                v_3_2 = a.v_0_2 * b.v_3_0 + a.v_1_2 * b.v_3_1 + a.v_2_2 * b.v_3_2 + a.v_3_2 * b.v_3_3,
                v_3_3 = a.v_0_3 * b.v_3_0 + a.v_1_3 * b.v_3_1 + a.v_2_3 * b.v_3_2 + a.v_3_3 * b.v_3_3
            };
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            return Matrix.Multiply(a, b);
        }

        public static Vector Multiply(Matrix a, Vector b)
        {
            return new Vector
            {
                X = a.v_0_0 * b.X + a.v_1_0 * b.Y + a.v_2_0 * b.Z + a.v_3_0 * 1f,
                Y = a.v_0_1 * b.X + a.v_1_1 * b.Y + a.v_2_1 * b.Z + a.v_3_1 * 1f,
                Z = a.v_0_2 * b.X + a.v_1_2 * b.Y + a.v_2_2 * b.Z + a.v_3_2 * 1f
            };
        }

        public static Vector operator *(Matrix a, Vector b)
        {
            return Matrix.Multiply(a, b);
        }

        public static Matrix Rotation(Quaternion q)
        {
            double length2 = q.Length2;
            // normalize quat if required.
            // We can avoid the expensive sqrt in this case since all 'coefficients' below are products of two q components.
            // That is a square of a square root, so it is possible to avoid that
            double rlength2 = length2 != 1.0
                ? 2.0 / length2
                : 2.0;

            double x2 = rlength2 * q.X;
            double y2 = rlength2 * q.Y;
            double z2 = rlength2 * q.Z;

            double xx = q.X * x2;
            double xy = q.X * y2;
            double xz = q.X * z2;

            double yy = q.Y * y2;
            double yz = q.Y * z2;
            double zz = q.Y * z2;

            double wx = q.W * x2;
            double wy = q.W * y2;
            double wz = q.W * z2;

            var result = new Matrix
            {
                //v_0_0 = 1-(yy+zz), v_1_0 = xy - wz,   v_2_0 = xz + wz,   v_3_0 = 0,
                //v_0_1 = xy + wz,   v_1_1 = 1-(xx+zz), v_2_1 = yz - wx,   v_3_1 = 0,
                //v_0_2 = xz - wy,   v_1_2 = yz + wx,   v_2_2 = 1-(xx+yy), v_3_2 = 0,
                //v_0_3 = 0,         v_1_3 = 0,         v_2_3 = 0,         v_3_3 = 1 

                v_0_0 = 1.0 - (yy + zz),
                v_1_0 = xy - wz,
                v_2_0 = xz + wy,
                v_3_0 = 0,

                v_0_1 = xy + wz,
                v_1_1 = 1.0 - (xx + zz),
                v_2_1 = yz - wx,
                v_3_1 = 0,

                v_0_2 = xz - wy,
                v_1_2 = yz + wx,
                v_2_2 = 1.0 - (xx + yy),
                v_3_2 = 0,

                v_0_3 = 0,
                v_1_3 = 0,
                v_2_3 = 0,
                v_3_3 = 1
            };

            return result;
        }

        public static Matrix Translation(Vector v)
        {
            return new Matrix
            {
                v_0_0 = 1,
                v_1_0 = 0,
                v_2_0 = 0,
                v_3_0 = v.X,

                v_0_1 = 0,
                v_1_1 = 1,
                v_2_1 = 0,
                v_3_1 = v.Y,

                v_0_2 = 0,
                v_1_2 = 0,
                v_2_2 = 1,
                v_3_2 = v.Z,

                v_0_3 = 0,
                v_1_3 = 0,
                v_2_3 = 0,
                v_3_3 = 1
            };
        }

        public static Matrix Scale(Vector v)
        {
            return new Matrix
            {
                v_0_0 = v.X,
                v_1_0 = 0,
                v_2_0 = 0,
                v_3_0 = 0,

                v_0_1 = 0,
                v_1_1 = v.Y,
                v_2_1 = 0,
                v_3_1 = 0,

                v_0_2 = 0,
                v_1_2 = 0,
                v_2_2 = v.Z,
                v_3_2 = 0,

                v_0_3 = 0,
                v_1_3 = 0,
                v_2_3 = 0,
                v_3_3 = 1
            };
        }
    }
}
