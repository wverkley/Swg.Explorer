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

                skeletonFile.Skeletons.First().Bones.First(b => b.Name == "root").ToString(Console.Out);
                Console.WriteLine();
                skeletonFile.Skeletons.First().Bones.First(b => b.Name == "lThigh").ToString(Console.Out);
                Console.WriteLine();
                skeletonFile.Skeletons.First().Bones.First(b => b.Name == "rThigh").ToString(Console.Out);

                new ColladaSkeletonExporter(repository, skeletonFile).Export(@"C:\Users\user\Desktop\all_b.skt");
            }
        }

        static void Main()
        {
            TestSkeleton();
            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
