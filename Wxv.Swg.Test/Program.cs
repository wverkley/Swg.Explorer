using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna;
using Microsoft.Xna.Framework;

//using Wxv.Swg.Common;

namespace Wxv.Swg.Test
{
    public class Program
    {
        static Matrix TestBoneTransform(Matrix parentMatrix, Quaternion preRotation, Quaternion postRotation, Vector3 offset)
        {
            //var rotation = preRotation * postRotation;
            //var localMatrix = Matrix.Rotation(rotation);

            var rotation = postRotation;

            var localMatrix = Matrix.CreateFromQuaternion(preRotation) * Matrix.CreateFromQuaternion(postRotation) * Matrix.CreateTranslation(offset);

            var result = localMatrix;

            return result;
        }

        static void TestBoneTransform()
        {
            /*
             
    Name=root, 
    ParentIndex=-1, 
    PreRotation=X:1.000,Y:0.000,Z:0.000,W:0.000, 
    PostRotation=X:1.000,Y:0.000,Z:0.000,W:0.000, 
    Offset=X:0.000,Y:1.002,Z:-0.054,
    Position=X:0.000,Y:1.002,Z:-0.054

    Name=lThigh, 
    ParentIndex=0, 
    PreRotation=X:1.000,Y:0.000,Z:0.000,W:0.000, 
    PostRotation=X:1.000,Y:0.000,Z:0.000,W:0.000, 
    Offset=X:-0.076,Y:-0.038,Z:0.037,
    Position=X:-0.076,Y:0.965,Z:-0.017
    
    
    Name=rThigh, 
    ParentIndex=0, 
    PreRotation=X:1.000,Y:0.000,Z:0.000,W:0.000, 
    PostRotation=X:0.000,Y:-1.000,Z:0.000,W:0.000, 
    Offset=X:0.076,Y:-0.038,Z:0.037,
    Position=X:-0.076,Y:1.040,Z:-0.017    
             */

            var rootPreRotation = new Quaternion { X = 1, Y = 0, Z = 0, W = 0 };
            var rootPostRotation = new Quaternion { X = 1, Y = 0, Z = 0, W = 0 };
            var rootOffset = new Vector3 { X = -0f, Y = -1.002f, Z = 0.054f };

            var lThighPreRotation = new Quaternion { X = 1, Y = 0, Z = 0, W = 0 };
            var lThighPostRotation = new Quaternion { X = 1, Y = 0, Z = 0, W = 0 };
            var lThighOffset = new Vector3 { X = -0.076f, Y = -0.038f, Z = 0.037f };

            var rThighPreRotation = new Quaternion { X = 1, Y = 0, Z = 0, W = 0 };
            var rThighPostRotation = new Quaternion { X = 0, Y = -1, Z = 0, W = 0 };
            var rThighOffset = new Vector3 { X = 0.076f, Y = -0.038f, Z = 0.037f };

            var rootTransform = TestBoneTransform(Matrix.Identity, rootPreRotation, rootPostRotation, rootOffset);
            var rootPosition = Vector3.Transform(Vector3.Zero, rootTransform);
            Console.WriteLine("root: {0}->{1}", rootOffset, rootPosition);

            var lThighTransform = TestBoneTransform(rootTransform, lThighPreRotation, lThighPostRotation, lThighOffset);
            var lThighPosition = Vector3.Transform(Vector3.Zero, lThighTransform);
            Console.WriteLine("lThigh: {0}->{1}", lThighOffset, lThighPosition);

            var rThighTransform = TestBoneTransform(rootTransform, rThighPreRotation, rThighPostRotation, rThighOffset);
            var rThighPosition = Vector3.Transform(Vector3.Zero, rThighTransform);
            Console.WriteLine("rThigh: {0}->{1}", rThighOffset, rThighPosition);

        }

        static void Main()
        {
            //Console.WriteLine("Test: {0}", 
            //    //Matrix.Identity
            //     Matrix.Scale(new Vector3 { X = 10, Y = 10, Z = 10 })
            //    * Matrix.Translation(new Vector3 { X = 1, Y = 2, Z = 3 })
            //    * Vector3.Zero);
            //Console.WriteLine("Test: {0}",
            //    //Matrix.Identity
            //    Matrix.Translation(new Vector3 { X = 1, Y = 2, Z = 3 })
            //    * Matrix.Scale(new Vector3 { X = 10, Y = 10, Z = 10 })
            //    * Vector3.Zero);


            TestBoneTransform();


            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
