using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna;
using Microsoft.Xna.Framework;

using Wxv.Swg.Common;
using Wxv.Swg.Common.Files;
using Wxv.Swg.Common.Exporters;

namespace Wxv.Swg.Test
{
    public class Program
    {
        static void TestSkeleton()
        {
            using (var repository = Repository.Load(@"c:\swgemu"))
            {
                var skeletonFile = repository.Load<SkeletonFile>(
                    @"appearance/skeleton/all_b.skt", 
                    stream => new SkeletonFileReader().Load(stream));

                //foreach(var b in skeletonFile.Skeletons.First().Bones
                //    .Where(b => b.Parent == null || (b.Parent.Parent == null) || (b.Parent.Parent.Parent == null)))
                //{
                //    b.ToString(Console.Out); 
                //    Console.WriteLine();
                //}

                //skeletonFile.Skeletons.First().Bones.First(b => b.Name == "root").ToString(Console.Out);
                //Console.WriteLine();
                //skeletonFile.Skeletons.First().Bones.First(b => b.Name == "lThigh").ToString(Console.Out);
                //Console.WriteLine();
                //skeletonFile.Skeletons.First().Bones.First(b => b.Name == "rThigh").ToString(Console.Out);

                skeletonFile.Skeletons.First().ToString(Console.Out);

                new ColladaSkeletonExporter(repository, skeletonFile).Export(@"C:\Users\user\Desktop\all_b.dae");
            }
        }

        static void TestQuat()
        {
            var offset = new Vector3 { X = 2, Y = 2, Z = 2 };
            var preRotation = new Quaternion { X = 0, Y = 0, Z = 0, W = 1 };
            var postRotation = new Quaternion { X = -1, Y = 0, Z = 0, W = 0 };

            var offsetMatrix = Matrix.CreateTranslation(offset);
            var matrixPreRotation = Matrix.CreateFromQuaternion(preRotation);
            var matrixPostRotation = Matrix.CreateFromQuaternion(postRotation);
            var m = offsetMatrix * matrixPreRotation * matrixPostRotation;

            Console.WriteLine("aa * r :  " + Vector3.Transform(Vector3.Zero, m).ToFormatString());
            Console.WriteLine();
        }

        static void Main()
        {
            TestQuat();
            TestSkeleton();
            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
